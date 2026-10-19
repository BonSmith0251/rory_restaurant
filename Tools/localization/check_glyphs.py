"""Checks that every character of each translation exists in that language's fallback font.
Usage (from the project folder): python Tools/localization/check_glyphs.py"""
import struct, sys, os
def cmap(path):
    d=open(path,'rb').read()
    n=struct.unpack('>H',d[4:6])[0]; tables={}
    for i in range(n):
        tag,cs,off,ln=struct.unpack('>4sIII',d[12+16*i:28+16*i]); tables[tag]=off
    o=tables[b'cmap']; cnt=struct.unpack('>H',d[o+2:o+4])[0]; chars=set()
    for i in range(cnt):
        pid,eid,so=struct.unpack('>HHI',d[o+4+8*i:o+12+8*i]); s=o+so; fmt=struct.unpack('>H',d[s:s+2])[0]
        if fmt==4:
            segx2=struct.unpack('>H',d[s+6:s+8])[0]; seg=segx2//2
            ends=struct.unpack('>%dH'%seg,d[s+14:s+14+segx2]); st=s+16+segx2
            starts=struct.unpack('>%dH'%seg,d[st:st+segx2]); dl=st+segx2
            deltas=struct.unpack('>%dh'%seg,d[dl:dl+segx2]); ro=dl+segx2
            ros=struct.unpack('>%dH'%seg,d[ro:ro+segx2])
            for k in range(seg):
                for c in range(starts[k],ends[k]+1):
                    if c==0xFFFF: continue
                    if ros[k]==0: g=(c+deltas[k])&0xFFFF
                    else:
                        gi=ro+2*k+ros[k]+2*(c-starts[k]); g=struct.unpack('>H',d[gi:gi+2])[0]
                        if g: g=(g+deltas[k])&0xFFFF
                    if g: chars.add(c)
        elif fmt==12:
            ng=struct.unpack('>I',d[s+12:s+16])[0]
            for k in range(ng):
                a,b,g=struct.unpack('>III',d[s+16+12*k:s+28+12*k])
                chars.update(range(a,b+1))
    return chars
fonts={'zh-Hans':'ZCOOLKuaiLe-Regular.ttf','ja':'MPLUSRounded1c-Regular.ttf','ko':'Jua-Regular.ttf'}
src='Assets/Rory/Localization/Source'
for code,f in fonts.items():
    cs=cmap('Assets/Rory/Fonts/CJK/'+f)
    used=set()
    for t in ['UI','Game','Dialogue','Events']:
        lines=open(f'{src}/{t}.tsv',encoding='utf-8').read().split('\n')
        h=lines[0].split('\t'); ci=h.index(code)
        for l in lines[1:]:
            if l: used.update(l.split('\t')[ci])
    missing=sorted(c for c in used if ord(c)>127 and ord(c) not in cs)
    print(code, f, 'glyphs', len(cs), 'used', len(used), 'missing', ''.join(missing))
