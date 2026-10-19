// Ported from CubicHermiteInteropolation.BezierPath.as (decompiled).
// Points passed to init/addPoints are AS3 object literals {x, y}: List<object> of Dictionary<string, object>
// (Point or any object with x/y fields/properties is also accepted).
using System;
using System.Collections.Generic;
using Starlite;
using Mathf = UnityEngine.Mathf;

public class BezierPath : Sprite
{
    public const float _RAD2DEG = 57.29577951308232f;
    public Global _global;
    public List<object> _points;
    public object _lastPoint;
    public List<Dictionary<string, object>> _plotPoints;
    public float time = 0;
    public float delay;          // AS3 NaN default; GreenSock treats a NaN delay as 0
    public Ease ease;
    public float positionCount = 0;
    public float prevPosition = 0;
    public float strokeThickness = 0.5f;
    public DisplayObject _follower;
    public bool tweenPS = false;

    public BezierPath()
    {
        this._global = Global.getInstance();
        this._plotPoints = new List<Dictionary<string, object>>();
    }

    public void init(List<object> arg1)
    {
        this._points = arg1;
        this._lastPoint = arg1[0];
        this._plotPoints = new List<Dictionary<string, object>>();
        double t = 0; // AS3 Number (double) accumulation, keeps the original point count
        while (t < 1)
        {
            this._plotPoints.Add(this.plotPoint((float)t));
            t = t + 0.0005;
        }
    }

    public void addPoints(List<object> arg1)
    {
        this._points = arg1;
        double t = 0; // AS3 Number (double) accumulation, keeps the original point count
        while (t < 1)
        {
            this._plotPoints.Add(this.plotPoint((float)t));
            t = t + 0.0005;
        }
    }

    public void start(float arg1, int arg2, int arg3, Ease arg4 = null, bool arg5 = true)
    {
        float time = arg1;
        int targetID = arg2;
        int UIIdx = arg3;
        bool removeCustomer = arg5;
        if (arg4 == null)
        {
            this.ease = Quint.easeOut;
        }
        this._global._minigameHO.tweenAr.set(this._global._minigameHO.getTween(), TweenLite.to(this, time, new Vars
        {
            ["positionCount"] = this._plotPoints.Count,
            ["delay"] = this.delay,
            ["ease"] = this.ease,
            ["onUpdate"] = (Action<Event>)this.draw,
            ["onComplete"] = (Action<int, int, bool>)((f1_arg1, f1_arg2, f1_arg3) =>
            {
                int _targetIdx = f1_arg1;
                int UIidx = f1_arg2;
                bool removeCust = f1_arg3;
                this._global._minigameHO.tweenAr.set(this._global._minigameHO.getTween(), TweenLite.delayedCall(0.2f, (Action<int, int, bool>)((f2_arg1, f2_arg2, f2_arg3) =>
                {
                    if (!this._global._minigameHO.ready)
                    {
                        return;
                    }
                    if (f2_arg3)
                    {
                        this._global._minigameHO._HOClass.removeItem(f2_arg1, f2_arg2);
                    }
                    else
                    {
                        this._global._minigameHO._HOClass.removeMultipleItem(f2_arg1, f2_arg2);
                    }
                }), new object[] { _targetIdx, UIidx, removeCust }));
            }),
            ["onCompleteParams"] = new object[] { targetID, UIIdx, removeCustomer },
        }));
    }

    public void startPS(float arg1, Ease arg2 = null)
    {
        float time = arg1;
        this.tweenPS = true;
        if (arg2 == null)
        {
            // (sic) the original only assigns the (null) argument
            this.ease = arg2;
        }
        this._global._minigameHO.tweenAr.set(this._global._minigameHO.getTween(), TweenLite.to(this, time, new Vars
        {
            ["positionCount"] = this._plotPoints.Count,
            ["delay"] = this.delay,
            ["ease"] = this.ease,
            ["onUpdate"] = (Action<Event>)this.draw,
            ["onComplete"] = (Action)(() =>
            {
                this._global.ps.addEventListener(Event.ENTER_FRAME, this._global._minigameHO._HOClass.tweenCircle);
            }),
        }));
    }

    public void kill()
    {
        TweenLite.killTweensOf(this);
    }

    public void startParticleTween(float arg1, Ease arg2 = null)
    {
        int count = 0;
        if (arg2 == null)
        {
            this.ease = arg2;
        }
        this.tweenParticle(arg1, this.ease, this, count);
    }

    public void tweenParticle(float arg1, Ease arg2, Sprite arg3, int arg4)
    {
        float time = arg1;
        Ease ease = arg2;
        Sprite target = arg3;
        int count = arg4;
        if (count > 3)
        {
            return;
        }
        this.positionCount = 0;
        this._global._minigameHO.tweenAr.set(this._global._minigameHO.getTween(), TweenLite.to(this, time, new Vars
        {
            ["positionCount"] = this._plotPoints.Count,
            ["delay"] = this.delay,
            ["ease"] = Linear.easeNone,
            ["onUpdate"] = (Action<Event>)this.drawParticle,
            ["onComplete"] = (Action<float, Ease, Sprite, int>)((f1_arg1, f1_arg2, f1_arg3, f1_arg4) =>
            {
                f1_arg4++;
                this.tweenParticle(f1_arg1, f1_arg2, f1_arg3, f1_arg4);
            }),
            ["onCompleteParams"] = new object[] { time, ease, target, count },
        }));
    }

    public void changePoints()
    {
        this.init(this._points);
    }

    public void drawParticle(Event arg1 = null)
    {
        this.positionCount = Mathf.Floor(this.positionCount);
        // (the original's stroke-drawing loop over _plotPoints has an empty body: graphics calls were stripped)
        if (this._follower != null)
        {
            this.dragFollowerParticle();
        }
        this.prevPosition = this.positionCount;
    }

    public void dragFollowerParticle()
    {
        int pc = (int)this.positionCount;
        if (this.positionCount < this._plotPoints.Count)
        {
            this._follower.x = (float)this._plotPoints[pc]["x"];
            this._follower.y = (float)this._plotPoints[pc]["y"];
            if (this.positionCount > 0)
            {
                (this._follower as PDParticleSystem).emitAngle = Mathf.Atan2(
                    (float)this._plotPoints[pc]["y"] - (float)this._plotPoints[pc - 1]["y"],
                    (float)this._plotPoints[pc]["x"] - (float)this._plotPoints[pc - 1]["x"]) - Mathf.PI;
            }
        }
    }

    public void draw(Event arg1 = null)
    {
        this.positionCount = Mathf.Floor(this.positionCount);
        // (the original's stroke-drawing loop over _plotPoints has an empty body: graphics calls were stripped)
        if (this._follower != null)
        {
            this.dragFollower();
        }
        this.prevPosition = this.positionCount;
    }

    public void dragFollower()
    {
        int pc = (int)this.positionCount;
        if (this.positionCount < this._plotPoints.Count)
        {
            if (!this.tweenPS)
            {
                this._follower.x = (float)this._plotPoints[pc]["x"];
                this._follower.y = (float)this._plotPoints[pc]["y"];
            }
            else
            {
                (this._follower as PDParticleSystem).emitterX = (float)this._plotPoints[pc]["x"];
                (this._follower as PDParticleSystem).emitterY = (float)this._plotPoints[pc]["y"];
            }
        }
    }

    public DisplayObject follower
    {
        set { this._follower = value; }
    }

    public Dictionary<string, object> plotPoint(float arg1)
    {
        float x = (1 - arg1) * (1 - arg1) * (1 - arg1) * PX(this._points[0]);
        float y = (1 - arg1) * (1 - arg1) * (1 - arg1) * PY(this._points[0]);
        float stroke = (1 - arg1) * (1 - arg1) * (1 - arg1) * 0.5f;
        if (this._points.Count > 1)
        {
            x = x + 3 * ((1 - arg1) * (1 - arg1)) * arg1 * PX(this._points[1]);
            y = y + 3 * ((1 - arg1) * (1 - arg1)) * arg1 * PY(this._points[1]);
            stroke = 3 * ((1 - arg1) * (1 - arg1)) * arg1 * 5;
        }
        if (this._points.Count > 2)
        {
            x = x + 3 * (1 - arg1) * (arg1 * arg1) * PX(this._points[2]);
            y = y + 3 * (1 - arg1) * (arg1 * arg1) * PY(this._points[2]);
            stroke = 3 * (1 - arg1) * (arg1 * arg1) * 5;
        }
        if (this._points.Count > 3)
        {
            x = x + arg1 * arg1 * arg1 * PX(this._points[3]);
            y = y + arg1 * arg1 * arg1 * PY(this._points[3]);
            stroke = arg1 * arg1 * arg1 * 0.5f;
        }
        if (this._points.Count > 4)
        {
            x = x + (1 - arg1) * arg1 * arg1 * arg1 * PX(this._points[4]);
            y = y + (1 - arg1) * arg1 * arg1 * arg1 * PY(this._points[4]);
            stroke = arg1 * arg1 * arg1 * 0.5f;
        }
        return new Dictionary<string, object> { ["x"] = x, ["y"] = y, ["stroke"] = stroke };
    }

    // ---- helpers: read x/y of an AS3 object literal ({x, y} dictionary), Point, or any object with x/y members
    static float PX(object p) => PointField(p, "x");
    static float PY(object p) => PointField(p, "y");

    static float PointField(object p, string key)
    {
        if (p is IDictionary<string, object> d) return d.TryGetValue(key, out var v) ? AS3.toNumber(v) : 0;
        if (p is Point pt) return key == "x" ? pt.x : pt.y;
        return p == null ? 0 : PropertyAccess.Get(p, key);
    }
}
