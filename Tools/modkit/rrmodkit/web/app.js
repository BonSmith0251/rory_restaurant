"use strict";
const $ = (s, el = document) => el.querySelector(s);
const esc = s => String(s ?? "").replace(/[&<>"']/g, c => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[c]));
const enc = encodeURIComponent;
const fmtKB = n => n >= 1048576 ? (n / 1048576).toFixed(2) + " MB" : (n / 1024).toFixed(1) + " KB";
let STATE = null, CURRENT = null, BUST = Date.now();

async function api(path, opts = {}) {
  document.body.classList.add("busy");
  try {
    const r = await fetch(path, opts);
    const ct = r.headers.get("content-type") || "";
    const body = ct.includes("json") ? await r.json() : await r.blob();
    if (!r.ok) throw new Error(body.error || r.statusText);
    return body;
  } finally { document.body.classList.remove("busy"); }
}
const post = (path, data, raw) => api(path, {
  method: "POST", body: raw ? data : JSON.stringify(data ?? {}),
  headers: raw ? { "Content-Type": "application/octet-stream" } : { "Content-Type": "application/json" }
});

function toast(msg, err) {
  const d = document.createElement("div");
  d.textContent = msg; if (err) d.className = "e";
  $("#toast").appendChild(d);
  setTimeout(() => d.remove(), err ? 9000 : 3500);
}
function dialog(title, body) { $("#dlgTitle").textContent = title; $("#dlgBody").textContent = body; $("#dlg").showModal(); }
const guard = fn => async (...a) => { try { return await fn(...a); } catch (e) { toast(e.message, true); } };

function pickFile(accept) {
  return new Promise(res => {
    const f = $("#filePick"); f.value = ""; f.accept = accept || "";
    f.onchange = () => res(f.files[0] || null); f.click();
  });
}
const readFile = file => file.arrayBuffer();
function enableDrop(el, onFile) {
  el.classList.add("drop");
  el.addEventListener("dragover", e => { e.preventDefault(); el.classList.add("over"); });
  el.addEventListener("dragleave", () => el.classList.remove("over"));
  el.addEventListener("drop", e => { e.preventDefault(); el.classList.remove("over"); const f = e.dataTransfer.files[0]; if (f) onFile(f); });
}
function download(url) { const a = document.createElement("a"); a.href = url + (url.includes("?") ? "&" : "?") + "dl=1"; a.click(); }

// ------------------------------------------------------------------ sidebar
const ICONS = { atlas: "▦", image: "▣", sound: "♪", text: "≡", font: "A", binary: "◇" };
async function loadState() {
  STATE = await api("/api/state");
  $("#wsPath").textContent = STATE.workspace + (STATE.installed.length ? `  ·  ${STATE.installed.length} file(s) installed in game` : "");
  renderTree();
}
function renderTree() {
  const q = $("#search").value.toLowerCase(), only = $("#onlyMod").checked;
  const open = new Set([...document.querySelectorAll(".cat[open]")].map(d => d.dataset.name));
  const first = !$("#tree .cat");
  $("#tree").innerHTML = STATE.categories.map(c => {
    const items = c.items.filter(i => (!q || (i.label + " " + (i.rel || "")).toLowerCase().includes(q)) && (!only || i.status !== "clean"));
    if (!items.length) return "";
    const nmod = c.items.filter(i => i.status !== "clean").length;
    const isOpen = q || only || open.has(c.name) || (first && c.name !== "Unused loose copies");
    return `<details class="cat" data-name="${esc(c.name)}" ${isOpen ? "open" : ""}><summary><span>${esc(c.name)}</span><span>${nmod ? nmod + " ● " : ""}${c.items.length}</span></summary>` +
      items.map(i => `<div class="item ${CURRENT && CURRENT.id === i.id ? "active" : ""}" data-id="${esc(i.id)}" title="${esc(i.rel || i.label)}">
        <span class="ico">${ICONS[i.type] || "·"}</span><span class="lbl">${esc(i.label)}</span>${i.detail ? `<span class="det">${esc(i.detail)}</span>` : ""}<span class="dot ${i.status}"></span></div>`).join("") + "</details>";
  }).join("");
}
$("#tree").addEventListener("click", e => {
  const it = e.target.closest(".item"); if (!it) return;
  const item = STATE.categories.flatMap(c => c.items).find(i => i.id === it.dataset.id);
  open(item);
});
$("#search").addEventListener("input", renderTree);
$("#onlyMod").addEventListener("change", renderTree);

function syncBadge(h) {
  const it = STATE.categories.flatMap(c => c.items).find(i => i.h === h);
  const b = $(".vhead h2 .badge");
  if (it && b) { b.textContent = it.status; b.className = "badge " + it.status; }
}
async function refresh() { await loadState(); if (CURRENT) open(STATE.categories.flatMap(c => c.items).find(i => i.id === CURRENT.id) || CURRENT, true); }

const open = guard(async (item, keep) => {
  CURRENT = item; BUST = Date.now();
  document.querySelectorAll(".item").forEach(el => el.classList.toggle("active", el.dataset.id === item.id));
  if (item.type === "atlas") return viewAtlas(item, keep);
  const info = await api("/api/item?h=" + enc(item.h));
  ({ image: viewImage, sound: viewSound, text: viewText, font: viewFont, binary: viewBinary })[info.type](item, info);
});

function head(title, status, meta, buttons) {
  return `<div class="vhead"><h2>${esc(title)} <span class="badge ${status}">${esc(status)}</span></h2>${buttons || ""}${meta ? `<div class="meta">${meta}</div>` : ""}</div>`;
}
const rawUrl = h => `/api/raw?h=${enc(h)}&v=${BUST}`;

async function doReplace(h, file) {
  const res = await post("/api/replace?h=" + enc(h), await readFile(file), true);
  toast("Replaced with " + file.name); await refresh(); return res;
}
async function doRevert(h) {
  if (!confirm("Revert this asset to the original game version? Your edits to it are lost.")) return;
  await post("/api/revert?h=" + enc(h)); toast("Reverted"); await refresh();
}
function commonButtons(h, accept) {
  return `<button data-b="dl">Download</button><button data-b="rep">Replace…</button><button data-b="rev" class="danger">Revert</button>`;
}
function wireCommon(root, h, accept) {
  $("[data-b=dl]", root).onclick = () => download(rawUrl(h));
  $("[data-b=rep]", root).onclick = guard(async () => { const f = await pickFile(accept); if (f) await doReplace(h, f); });
  $("[data-b=rev]", root).onclick = guard(() => doRevert(h));
}

// ------------------------------------------------------------------ image / sound / binary
function viewImage(item, info) {
  const dims = info.w ? `${info.w}×${info.h_px}` : "";
  const kind = info.kind === "ean" ? "Encrypted texture (.ean), edited here as PNG" : info.kind === "eag" ? "Background (.eag = JPEG). Replace with a JPEG, or a PNG for transparency/lossless" : (info.tag || "image");
  $("#main").innerHTML = `<div class="view">${head(item.label, info.status, `${esc(kind)} · ${dims} · ${info.format || ""} · ${fmtKB(info.size)} · <code>${esc(info.work)}</code>${info.has_override ? " · <b>PNG override active</b>" : ""}`, commonButtons())}
    <div class="vbody imgview"><div class="frame checker"><img src="${rawUrl(item.h)}" alt=""></div><p class="hint">Drop an image file here to replace it.</p></div></div>`;
  wireCommon($("#main"), item.h, info.kind === "ean" ? "image/png" : "image/png,image/jpeg");
  enableDrop($(".imgview"), guard(f => doReplace(item.h, f)));
}
function viewSound(item, info) {
  const m = info.mp3 || {};
  $("#main").innerHTML = `<div class="view">${head(item.label, info.status, `MPEG ${m.mpeg || "?"} layer ${m.layer || "?"} · ${m.bitrate || "?"} kbps · ${m.sample_rate || "?"} Hz ${m.channels || ""} · ${(m.duration || 0).toFixed(2)} s · ${fmtKB(info.size)}`, commonButtons())}
    <div class="vbody imgview"><audio controls src="${rawUrl(item.h)}" style="width:min(600px,100%)"></audio>
    <p class="hint">Replace with an MP3 (44.1 / 22.05 kHz work best in Flash). Drop a file here to replace it.</p></div></div>`;
  wireCommon($("#main"), item.h, "audio/mpeg,.mp3");
  enableDrop($(".imgview"), guard(f => doReplace(item.h, f)));
}
function viewBinary(item, info) {
  $("#main").innerHTML = `<div class="view">${head(item.label, info.status, `${fmtKB(info.size)} · <code>${esc(info.work)}</code>`, commonButtons())}
    <div class="vbody imgview"><p class="hint">Binary file: download, edit with an external tool, then replace it.</p></div></div>`;
  wireCommon($("#main"), item.h, "");
}

// ------------------------------------------------------------------ text + table
function viewText(item, info, extraTabs) {
  const tabs = [];
  if (extraTabs) tabs.push(...extraTabs);
  if (info.table_tags && info.table_tags.length) tabs.push(["table", "Table"]);
  tabs.push(["src", "Source"]);
  $("#main").innerHTML = `<div class="view">${head(item.label, info.status, `<code>${esc(info.work)}</code> · ${fmtKB(info.size)} ${info.xml_error ? `· <span class="err">XML error: ${esc(info.xml_error)}</span>` : ""}`, commonButtons())}
    <div class="tabs">${tabs.map(([k, l], i) => `<button data-t="${k}" class="${i ? "" : "on"}">${l}</button>`).join("")}</div>
    <div class="vbody" id="tabBody"></div></div>`;
  wireCommon($("#main"), item.h, ".xml,.fnt,.pex,.txt,text/xml");
  const show = k => {
    document.querySelectorAll(".tabs button").forEach(b => b.classList.toggle("on", b.dataset.t === k));
    if (k === "src") sourceEditor(item); else if (k === "table") tableEditor(item, info.table_tags[0]); else extraTabs.find(t => t[0] === k)[2]();
  };
  document.querySelectorAll(".tabs button").forEach(b => b.onclick = () => show(b.dataset.t));
  show(tabs[0][0]);
}

const sourceEditor = guard(async item => {
  const t = await api("/api/text?h=" + enc(item.h));
  let sha = t.sha;
  $("#tabBody").innerHTML = `<div class="view"><div class="vhead"><button class="primary" id="save">Save (Ctrl+S)</button><span id="xstat" class="${t.error ? "err" : "ok"}">${t.error ? "⚠ " + esc(t.error) : "✓ well-formed"}</span><span class="badge">${esc(t.encoding)}</span></div><textarea class="code" spellcheck="false"></textarea></div>`;
  const ta = $("#tabBody textarea"); ta.value = t.text;
  ta.addEventListener("keydown", e => {
    if (e.key === "Tab") { e.preventDefault(); const s = ta.selectionStart; ta.setRangeText("\t", s, ta.selectionEnd, "end"); }
    if ((e.ctrlKey || e.metaKey) && e.key === "s") { e.preventDefault(); save(); }
  });
  const save = guard(async () => {
    let r;
    try { r = await post("/api/text?h=" + enc(item.h), { text: ta.value, sha }); }
    catch (e) {
      if (/XML error/.test(e.message) && confirm(e.message + "\n\nSave anyway? The game may fail to load it.")) r = await post("/api/text?h=" + enc(item.h), { text: ta.value, sha, force: true });
      else throw e;
    }
    sha = r.sha; $("#xstat").className = "ok"; $("#xstat").textContent = "✓ saved"; toast("Saved"); await loadState(); syncBadge(item.h);
  });
  $("#save").onclick = save;
});

const tableEditor = guard(async (item, tag) => {
  const t = await api(`/api/table?h=${enc(item.h)}&tag=${enc(tag)}`);
  const edits = new Map(), ops = [];
  const cols = t.columns;
  $("#tabBody").innerHTML = `<div class="view"><div class="vhead">
      <label>Records: <select id="ttag">${t.tags.map(x => `<option ${x === t.tag ? "selected" : ""}>${esc(x)}</option>`).join("")}</select></label>
      <span class="hint">${t.rows.length} rows · edits are written in place, keeping comments and formatting</span>
      <span style="flex:1"></span><button id="tdiscard">Discard</button><button class="primary" id="tsave">Save changes</button></div>
    <div class="tablewrap"><table class="grid"><thead><tr><th>#</th>${t.has_context ? "<th>in</th>" : ""}${cols.map(c => `<th>${esc(c)}</th>`).join("")}<th></th></tr></thead><tbody>
    ${t.rows.map((r, i) => `<tr data-r="${i}"><td class="n">${i + 1}</td>${t.has_context ? `<td class="n">${esc(r.context)}</td>` : ""}${cols.map(c => c in r.fields
      ? `<td><input data-f="${esc(c)}" value="${esc(r.fields[c])}" size="${Math.min(60, Math.max(4, String(r.fields[c]).length + 1))}"></td>` : `<td></td>`).join("")}
      <td class="ops"><button data-op="duplicate" title="Duplicate row">⧉</button><button data-op="delete" title="Delete row">✕</button></td></tr>`).join("")}
    </tbody></table></div></div>`;
  $("#ttag").onchange = e => tableEditor(item, e.target.value);
  $("#tabBody tbody").addEventListener("input", e => {
    const inp = e.target, row = +inp.closest("tr").dataset.r, f = inp.dataset.f;
    edits.set(row + "\u0000" + f, { row, field: f, value: inp.value }); inp.classList.add("dirty");
  });
  $("#tabBody tbody").addEventListener("click", e => {
    const b = e.target.closest("button[data-op]"); if (!b) return;
    const tr = b.closest("tr"), row = +tr.dataset.r;
    ops.push({ op: b.dataset.op, row });
    if (b.dataset.op === "delete") tr.classList.add("del"); else { b.textContent = "⧉+"; }
    toast(`Row ${row + 1} will be ${b.dataset.op === "delete" ? "deleted" : "duplicated"} on save`);
  });
  $("#tdiscard").onclick = () => tableEditor(item, t.tag);
  $("#tsave").onclick = guard(async () => {
    if (!edits.size && !ops.length) return toast("Nothing to save");
    await post("/api/table?h=" + enc(item.h), { tag: t.tag, sha: t.sha, edits: [...edits.values()], row_ops: ops });
    toast(`Saved ${edits.size} change(s)${ops.length ? ` and ${ops.length} row operation(s)` : ""}`);
    await loadState(); syncBadge(item.h); tableEditor(item, t.tag);
  });
});

// ------------------------------------------------------------------ atlas / font sprite viewer
let ZOOM = 1;
function spriteStage(container, imgUrl, W, H, sprites, onSelect) {
  container.innerHTML = `<div class="stage checker"><img alt=""><canvas></canvas></div>`;
  const stage = $(".stage", container), img = $("img", stage), cv = $("canvas", stage), ctx = cv.getContext("2d");
  let hover = -1, sel = -1, showBoxes = true;
  img.src = imgUrl;
  const css = getComputedStyle(document.documentElement);
  function layout() {
    const w = Math.round(W * ZOOM), h = Math.round(H * ZOOM);
    img.style.width = w + "px"; img.style.height = h + "px";
    const dpr = window.devicePixelRatio || 1;
    cv.width = w * dpr; cv.height = h * dpr; cv.style.width = w + "px"; cv.style.height = h + "px";
    ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
    stage.classList.toggle("pixel", ZOOM > 1);
    draw();
  }
  function draw() {
    ctx.clearRect(0, 0, cv.width, cv.height);
    const accent = css.getPropertyValue("--accent").trim(), mod = css.getPropertyValue("--mod").trim();
    sprites.forEach((s, i) => {
      const on = i === sel || i === hover;
      if (!showBoxes && !on) return;
      ctx.lineWidth = on ? 2 : 1;
      ctx.strokeStyle = on ? accent : s.status === "modified" ? mod : "rgba(80,140,230,.55)";
      ctx.strokeRect(s.x * ZOOM + .5, s.y * ZOOM + .5, s.w * ZOOM - 1, s.h * ZOOM - 1);
      if (i === sel) { ctx.fillStyle = "rgba(200,83,30,.12)"; ctx.fillRect(s.x * ZOOM, s.y * ZOOM, s.w * ZOOM, s.h * ZOOM); }
    });
  }
  function hit(e) {
    const r = cv.getBoundingClientRect(), x = (e.clientX - r.left) / ZOOM, y = (e.clientY - r.top) / ZOOM;
    let best = -1, area = Infinity;
    sprites.forEach((s, i) => { if (x >= s.x && y >= s.y && x < s.x + s.w && y < s.y + s.h && s.w * s.h < area) { best = i; area = s.w * s.h; } });
    return best;
  }
  cv.addEventListener("mousemove", e => { const h = hit(e); if (h !== hover) { hover = h; cv.title = h >= 0 ? sprites[h].name : ""; draw(); } });
  cv.addEventListener("mouseleave", () => { hover = -1; draw(); });
  cv.addEventListener("click", e => { const h = hit(e); if (h >= 0) onSelect(h, false); });
  layout();
  return {
    select(i, scroll) {
      sel = i; draw();
      if (scroll && i >= 0) { const s = sprites[i], wrap = container; wrap.scrollTo({ left: s.x * ZOOM - wrap.clientWidth / 2 + s.w * ZOOM / 2, top: s.y * ZOOM - wrap.clientHeight / 2 + s.h * ZOOM / 2, behavior: "smooth" }); }
    },
    zoom(z) { ZOOM = z; layout(); },
    fit() { ZOOM = Math.min(1, (container.clientWidth - 32) / W, Math.max(0.25, (container.clientHeight - 32) / H)); layout(); },
    boxes(v) { showBoxes = v; draw(); },
  };
}

function zoomButtons() {
  return `<select id="zoom" title="Zoom"><option value="fit">Fit</option><option value="0.5">50%</option><option value="1" selected>100%</option><option value="2">200%</option><option value="4">400%</option></select>
    <label class="chk"><input type="checkbox" id="boxes" checked> boxes</label>`;
}
function wireZoom(st) {
  $("#zoom").onchange = e => e.target.value === "fit" ? st.fit() : st.zoom(+e.target.value);
  $("#boxes").onchange = e => st.boxes(e.target.checked);
  if (ZOOM !== 1) { const o = [...$("#zoom").options].find(o => +o.value === ZOOM); if (o) o.selected = true; }
}

const viewAtlas = guard(async (item, keep) => {
  const prevSel = keep && window._atlasSel;
  const a = await api("/api/atlas?xml=" + enc(item.rel));
  const nmod = a.sprites.filter(s => s.status !== "clean").length;
  const status = item.status;
  $("#main").innerHTML = `<div class="view">${head(item.label, status,
    `Texture <code>${esc(a.texture_h.slice(5))}</code> ${a.w}×${a.h} (${a.texture_status}) · XML ${a.xml_status} · ${a.sprites.length} sprites${nmod ? `, ${nmod} edited` : ""}${a.pending_new.length ? ` · <b>${a.pending_new.length} new sprite file(s) pending</b>` : ""}`,
    `${zoomButtons()}<button data-b="add">Add sprite…</button><button data-b="repack" title="Re-arrange all sprites into a fresh texture">Repack</button><button data-b="xml">Edit XML</button><button data-b="tex">Texture ▾</button><button data-b="rev" class="danger">Revert atlas</button>`)}
    <div class="atlas"><div class="stagewrap" id="sw"></div>
    <div class="side"><div class="filter"><input type="search" id="sfilter" placeholder="Filter ${a.sprites.length} sprites…"></div><div class="slist" id="slist"></div><div class="sdetail" id="sdet"><p class="hint">Click a sprite on the texture or in the list.</p></div></div></div></div>`;
  const sprites = a.sprites;
  const st = spriteStage($("#sw"), rawUrl(a.texture_h), a.w, a.h, sprites, (i) => select(i, false));
  wireZoom(st);
  const list = () => {
    const q = $("#sfilter").value.toLowerCase();
    $("#slist").innerHTML = sprites.map((s, i) => !q || s.name.toLowerCase().includes(q) ? `<div class="srow" data-i="${i}"><span class="dot ${s.status}"></span><span class="lbl">${esc(s.name)}</span><span class="det">${s.w}×${s.h}</span></div>` : "").join("");
  };
  list();
  $("#sfilter").oninput = list;
  $("#slist").onclick = e => { const r = e.target.closest(".srow"); if (r) select(+r.dataset.i, true); };
  function select(i, scroll) {
    window._atlasSel = sprites[i].name;
    st.select(i, scroll);
    document.querySelectorAll(".srow").forEach(r => r.classList.toggle("active", +r.dataset.i === i));
    const s = sprites[i], url = `/api/sprite?xml=${enc(item.rel)}&name=${enc(s.name)}&v=${BUST}`;
    $("#sdet").innerHTML = `<div class="prev checker"><img src="${url}" alt=""></div>
      <div class="kv"><b>name</b><span>${esc(s.name)}</span><b>rect</b><span>${s.x}, ${s.y} · ${s.w}×${s.h}</span>
      ${s.frame ? `<b>trim frame</b><span>${s.frame.join(", ")}</span>` : ""}<b>status</b><span>${s.status}</span></div>
      <div class="btns"><button data-s="dl">Export PNG</button><button data-s="rep" class="primary">Replace…</button><button data-s="rev" class="danger">Revert</button></div>
      <p class="hint">Same size → painted into place. Different size → the atlas is repacked automatically.${s.frame ? " This sprite is trimmed: keep its size to preserve the frame offset." : ""} You can also drop a PNG here.</p>`;
    $("[data-s=dl]").onclick = () => download(url);
    const rep = guard(async f => {
      if (!f) return;
      const r = await post(`/api/sprite?xml=${enc(item.rel)}&name=${enc(s.name)}`, await readFile(f), true);
      toast(r.report && r.report.repacked ? `Sprite replaced, atlas repacked to ${r.report.size}` : "Sprite replaced");
      (r.report?.warnings || []).forEach(w => toast(w, true));
      await refresh();
    });
    $("[data-s=rep]").onclick = async () => rep(await pickFile("image/png"));
    $("[data-s=rev]").onclick = guard(async () => { await post(`/api/sprite/revert?xml=${enc(item.rel)}&name=${enc(s.name)}`); toast("Sprite reverted"); await refresh(); });
    enableDrop($("#sdet"), rep);
  }
  if (prevSel) { const i = sprites.findIndex(s => s.name === prevSel); if (i >= 0) select(i, false); }
  const root = $("#main");
  $("[data-b=add]", root).onclick = guard(async () => {
    const f = await pickFile("image/png"); if (!f) return;
    const name = prompt("Name for the new sprite (the game looks sprites up by name):", f.name.replace(/\.png$/i, ""));
    if (!name) return;
    const r = await post(`/api/sprite?xml=${enc(item.rel)}&name=${enc(name)}&new=1`, await readFile(f), true);
    toast(`Added "${name}"` + (r.report?.size ? `, atlas repacked to ${r.report.size}` : "")); await refresh();
  });
  $("[data-b=repack]", root).onclick = guard(async () => {
    if (!confirm("Repack the whole atlas? Sprite positions change (the XML is rewritten to match).")) return;
    const r = await post("/api/atlas/repack?xml=" + enc(item.rel)); toast("Repacked to " + (r.report?.size || "?")); await refresh();
  });
  $("[data-b=xml]", root).onclick = () => open({ ...item, id: "file:" + item.rel, type: "text", label: item.label + ".xml" });
  $("[data-b=tex]", root).onclick = () => open({ id: a.texture_h, h: a.texture_h, label: a.texture_h.split("/").pop(), type: "image", rel: a.texture_h.slice(5) });
  $("[data-b=rev]", root).onclick = guard(async () => {
    if (!confirm("Revert texture, XML and every sprite of this atlas to the original?")) return;
    await post("/api/revert?h=" + enc(item.h)); toast("Atlas reverted"); await refresh();
  });
});

function viewFont(item, info) {
  viewText(item, info, [["glyphs", "Glyphs", guard(async () => {
    const f = await api("/api/font?h=" + enc(item.h));
    if (!f.page_h) { $("#tabBody").innerHTML = `<p class="hint" style="padding:16px">No page image found for this font.</p>`; return; }
    $("#tabBody").innerHTML = `<div class="view"><div class="vhead"><span>${esc(f.info.face || "")} ${esc(f.info.size || "")}px · ${f.sprites.length} glyphs · page <code>${esc(f.page_h)}</code></span><span style="flex:1"></span>${zoomButtons()}<button id="openpage">Open page image</button></div>
      <div class="atlas"><div class="stagewrap" id="sw"></div><div class="side"><div class="slist" id="slist"></div></div></div></div>`;
    const st = spriteStage($("#sw"), rawUrl(f.page_h), f.w, f.h, f.sprites, i => sel(i, false));
    wireZoom(st);
    $("#slist").innerHTML = f.sprites.map((s, i) => `<div class="srow" data-i="${i}"><span class="lbl">${esc(s.name)}</span><span class="det">${s.x},${s.y} ${s.w}×${s.h}</span></div>`).join("");
    const sel = (i, scroll) => { st.select(i, scroll); document.querySelectorAll(".srow").forEach(r => r.classList.toggle("active", +r.dataset.i === i)); };
    $("#slist").onclick = e => { const r = e.target.closest(".srow"); if (r) sel(+r.dataset.i, true); };
    $("#openpage").onclick = () => open({ id: f.page_h, h: f.page_h, label: f.page_h.split(/[/#]/).pop(), type: "image" });
  })]]);
}

// ------------------------------------------------------------------ analyze
const viewAnalyze = guard(async target => {
  CURRENT = null; renderTree();
  $("#main").innerHTML = `<div class="view"><div class="vhead"><h2>Asset analysis</h2>
    <button data-t="workspace" class="${target === "workspace" ? "primary" : ""}">Workspace (your edits)</button><button data-t="game" class="${target === "game" ? "primary" : ""}">Installed game</button>
    <button id="rawrep">Text report</button></div><div class="vbody"><p class="hint" style="padding:16px">Analyzing…</p></div></div>`;
  document.querySelectorAll("[data-t]").forEach(b => b.onclick = () => viewAnalyze(b.dataset.t));
  const r = await api("/api/analyze?target=" + target);
  $("#rawrep").onclick = () => dialog("Analysis report", r.text);
  const cnt = l => r.summary[l] || 0;
  const inv = Object.values(r.inventory).reduce((a, v) => ({ c: a.c + v.count, b: a.b + v.bytes }), { c: 0, b: 0 });
  const gp = r.gameplay;
  const tbl = (cols, rows) => `<table><tr>${cols.map(c => `<th>${c}</th>`).join("")}</tr>${rows.join("")}</table>`;
  $("#main .vbody").innerHTML = `<div class="report">
    <div class="cards">
      <div class="card"><b>${inv.c}</b><span>files · ${fmtKB(inv.b)}</span></div>
      <div class="card"><b>${r.textures.length}</b><span>encrypted textures</span></div>
      <div class="card"><b>${r.atlases.reduce((a, x) => a + x.sprites, 0)}</b><span>sprites in ${r.atlases.length} atlases</span></div>
      <div class="card"><b>${gp.levels ? gp.levels.count : "?"}</b><span>levels</span></div>
      <div class="card"><b style="color:var(--err)">${cnt("error")}</b><span>errors</span></div>
      <div class="card"><b style="color:var(--mod)">${cnt("warn")}</b><span>warnings</span></div>
      <div class="card"><b style="color:var(--info)">${cnt("info")}</b><span>notes</span></div>
    </div>
    <h3>Issues</h3>
    ${r.issues.length ? tbl(["Level", "Where", "Message"], r.issues.map(i => `<tr><td><span class="lvl ${i.level}">${i.level}</span></td><td><code>${esc(i.where)}</code></td><td>${esc(i.msg)}</td></tr>`)) : "<p class='ok'>No issues found.</p>"}
    <h3>Gameplay data</h3>
    ${tbl(["Item", "Value"], Object.entries(gp).map(([k, v]) => `<tr><td>${esc(k)}</td><td><code>${esc(typeof v === "object" ? JSON.stringify(v) : v)}</code></td></tr>`))}
    <h3>Atlases</h3>
    ${tbl(["Atlas", "Texture", "Sprites", "Trimmed", "Coverage", "Overlaps"], r.atlases.map(a => `<tr><td>${esc(a.file)}</td><td>${esc(a.texture_size || "?")}</td><td class="num">${a.sprites}</td><td class="num">${a.trimmed}</td><td class="num">${a.coverage != null ? Math.round(a.coverage * 100) + "%" : "?"}</td><td class="num">${a.overlaps ?? ""}</td></tr>`))}
    <h3>Textures</h3>
    ${tbl(["File", "Size", "Bytes", "POT", "Transparent"], r.textures.map(t => `<tr><td>${esc(t.file)}</td><td>${t.w}×${t.h}</td><td class="num">${fmtKB(t.bytes)}</td><td>${t.pot ? "yes" : "no"}</td><td class="num">${t.alpha ? Math.round(t.alpha.transparent * 100) + "%" : ""}</td></tr>`))}
    <h3>Sounds</h3>
    ${tbl(["File", "Duration", "kbps", "Hz", "Ch", "Listed"], r.sounds.map(s => `<tr><td>${esc(s.file)}</td><td class="num">${(s.duration || 0).toFixed(2)}s</td><td class="num">${esc(s.bitrate)}</td><td class="num">${s.sample_rate || ""}</td><td>${s.channels || ""}</td><td>${s.listed ? "yes" : "<b>no</b>"}</td></tr>`))}
    ${r.swf && r.swf.embedded ? `<h3>SWF (v${r.swf.version}, ${r.swf.classes} classes, key <code>${esc(r.swf.key)}</code>)</h3>
    ${tbl(["ID", "Tag", "Symbol", "Info"], r.swf.embedded.map(a => `<tr><td class="num">${a.id}</td><td>${a.tag_name}</td><td>${esc(a.symbol)}</td><td>${a.w ? a.w + "×" + a.h : esc(a.ext)}</td></tr>`))}` : ""}
  </div>`;
});

// ------------------------------------------------------------------ code browser
const viewCode = guard(async () => {
  CURRENT = null; renderTree();
  $("#main").innerHTML = `<div class="view"><div class="vhead"><h2>ActionScript bytecode browser</h2><input type="search" id="strq" placeholder="Search string constants…"><span class="hint">Read-only. Disassembly of RorysRestaurant.swf</span></div>
    <div class="code" style="flex:1;min-height:0"><div class="classes"><input type="search" id="cq" placeholder="Filter classes…"><div class="clist" id="clist">Loading…</div></div><pre id="dis">Select a method.</pre></div></div>`;
  const classes = await api("/api/classes");
  const render = () => {
    const q = $("#cq").value.toLowerCase();
    $("#clist").innerHTML = classes.filter(c => !q || c.name.toLowerCase().includes(q)).slice(0, 400).map(c =>
      `<details><summary>${esc(c.name)}${c.super && c.super !== "Object" ? ` <span class="hint">: ${esc(c.super)}</span>` : ""}</summary>
      ${c.methods.map(m => `<div class="m" data-abc="${m.abc}" data-m="${m.m}">${esc(m.kind.replace("method", ""))} ${esc(m.name)} <span>(${m.size}b)</span></div>`).join("")}</details>`).join("");
  };
  render();
  $("#cq").oninput = render;
  $("#clist").onclick = guard(async e => {
    const m = e.target.closest(".m"); if (!m) return;
    document.querySelectorAll(".m.active").forEach(x => x.classList.remove("active")); m.classList.add("active");
    const d = await api(`/api/disasm?abc=${m.dataset.abc}&m=${m.dataset.m}`);
    $("#dis").textContent = (d.name ? "// " + d.name + "\n" : "") + d.text;
  });
  $("#strq").onchange = guard(async e => {
    const s = await api("/api/strings?q=" + enc(e.target.value));
    $("#dis").textContent = `// ${s.length} string constant(s) containing "${e.target.value}"\n` + s.join("\n");
  });
});

// ------------------------------------------------------------------ top bar actions
document.querySelector(".actions").addEventListener("click", guard(async e => {
  const b = e.target.closest("button[data-act]"); if (!b) return;
  const act = b.dataset.act;
  if (act === "analyze") return viewAnalyze("workspace");
  if (act === "code") return viewCode();
  if (act === "launch") { await post("/api/launch"); return toast("Starting the game…"); }
  if (act === "restore") {
    if (!confirm("Restore every original game file that RRModKit replaced? (Your workspace edits are kept.)")) return;
    const r = await post("/api/restore"); dialog("Restore originals", r.log); return refresh();
  }
  if (act === "install" && !confirm("Build your changes and install them into the game folder?\nOriginal files are backed up to modkit/backup first.")) return;
  b.disabled = true;
  try {
    const r = await post("/api/build", { install: act === "install" });
    dialog(r.ok ? (act === "install" ? "Installed into the game" : "Build finished") : "Build failed",
      (r.log || "") + (r.files.length ? "" : "\n(no changed files)") + (act === "build" && r.ok ? `\n\nOutput folder: ${r.build_dir}` : ""));
    await refresh();
  } finally { b.disabled = false; }
}));

loadState().catch(e => toast(e.message, true));
