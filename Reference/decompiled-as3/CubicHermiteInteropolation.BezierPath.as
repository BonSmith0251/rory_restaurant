package CubicHermiteInteropolation {
class BezierPath extends Sprite {
    static const _RAD2DEG:Number = 57.29577951308232;
    var _global:Global;
    var _points:Array;
    var _lastPoint:Object;
    var _plotPoints:Array;
    var time:Number = 0;
    var delay:Number;
    var ease:Ease;
    var positionCount:Number = 0;
    var prevPosition:Number = 0;
    var strokeThickness:Number = 0.5;
    var _follower:DisplayObject;
    var tweenPS:Boolean = false;

    // static initializer
    static {
        _RAD2DEG = 180 / Math.PI;
    }

    function BezierPath():void {
        this._global = Global.getInstance();
        this._plotPoints = [];
        super();
    }

    function init(arg1:Array):void {
        this._points = arg1;
        this._lastPoint = arg1[0];
        this._plotPoints = [];
        var _local2 = Number(0);
        goto L53;
      L31:
        this._plotPoints.push(this.plotPoint(_local2));
        _local2 = Number(_local2 + 0.0005);
      L53:
        if (_local2 < 1) goto L31;
    }

    function addPoints(arg1:Array):void {
        this._points = arg1;
        var _local2 = Number(0);
        goto L37;
      L15:
        this._plotPoints.push(this.plotPoint(_local2));
        _local2 = Number(_local2 + 0.0005);
      L37:
        if (_local2 < 1) goto L15;
    }

    function start(arg1:Number, arg2:int, arg3:int, arg4:Ease = null, arg5:Boolean = true):void {
        time = arg1;
        targetID = arg2;
        UIIdx = arg3;
        $ease = arg4;
        removeCustomer = arg5;
        if ($ease != null) goto L53;
        this.ease = Quint.easeOut;
      L53:
        this._global._minigameHO.tweenAr[this._global._minigameHO.getTween()] = TweenLite.to(this, time, {"positionCount": this._plotPoints.length, "delay": this.delay, "ease": this.ease, "onUpdate": this.draw, "onComplete": function(f1_arg1:int, f1_arg2:int, f1_arg3:Boolean):void {
            _targetIdx = f1_arg1;
            UIidx = f1_arg2;
            removeCustomer = f1_arg3;
            _global._minigameHO.tweenAr[_global._minigameHO.getTween()] = TweenLite.delayedCall(0.2, function(f2_arg1:int, f2_arg2:int, f2_arg3:Boolean):void {
                if (_global._minigameHO.ready) goto L14;
                return;
              L14:
                if (!f2_arg3) goto L38;
                _global._minigameHO._HOClass.removeItem(f2_arg1, f2_arg2);
                goto L53;
              L38:
                _global._minigameHO._HOClass.removeMultipleItem(f2_arg1, f2_arg2);
              L53:
            }, [_targetIdx, UIidx, removeCustomer]);
        }, "onCompleteParams": [targetID, UIIdx, removeCustomer]});
    }

    function startPS(arg1:Number, arg2:Ease = null):void {
        time = arg1;
        $ease = arg2;
        this.tweenPS = true;
        if ($ease != null) goto L38;
        this.ease = $ease;
      L38:
        this._global._minigameHO.tweenAr[this._global._minigameHO.getTween()] = TweenLite.to(this, time, {"positionCount": this._plotPoints.length, "delay": this.delay, "ease": this.ease, "onUpdate": this.draw, "onComplete": function():void {
            _global.ps.addEventListener(Event.ENTER_FRAME, _global._minigameHO._HOClass.tweenCircle);
        }});
    }

    function kill():void {
        TweenLite.killTweensOf(this);
    }

    function startParticleTween(arg1:Number, arg2:Ease = null):void {
        var _local3 = 0;
        if (arg2 != null) goto L16;
        this.ease = arg2;
      L16:
        this.tweenParticle(arg1, this.ease, this, _local3);
    }

    function tweenParticle(arg1:Number, arg2:Ease, arg3:Sprite, arg4:int):void {
        time = arg1;
        ease = arg2;
        target = arg3;
        count = arg4;
        if (!(count > 3)) goto L39;
        return;
      L39:
        this.positionCount = 0;
        this._global._minigameHO.tweenAr[this._global._minigameHO.getTween()] = TweenLite.to(this, time, {"positionCount": this._plotPoints.length, "delay": this.delay, "ease": Linear.easeNone, "onUpdate": this.drawParticle, "onComplete": function(f1_arg1:Number, f1_arg2:Ease, f1_arg3:Sprite, f1_arg4:int):void {
            f1_arg4++;
            tweenParticle(f1_arg1, f1_arg2, f1_arg3, f1_arg4);
        }, "onCompleteParams": [time, ease, target, count]});
    }

    function changePoints():void {
        this.init(this._points);
    }

    function drawParticle(arg1:Event = null):void {
        this.positionCount = Math.floor(this.positionCount);
        var _local2 = Number(0);
        goto L44;
      L25:
        if (!(_local2 < this._plotPoints.length)) goto L44;
        _local2 = Number(_local2 + 2);
      L44:
        if (_local2 < this.positionCount) goto L25;
        if (this._follower == null) goto L67;
        this.dragFollowerParticle();
      L67:
        this.prevPosition = this.positionCount;
    }

    function dragFollowerParticle():void {
        if (!(this.positionCount < this._plotPoints.length)) goto L154;
        this._follower.x = this._plotPoints[this.positionCount].x;
        this._follower.y = this._plotPoints[this.positionCount].y;
        if (!(this.positionCount > 0)) goto L154;
        (this._follower as PDParticleSystem).emitAngle = Math.atan2(this._plotPoints[this.positionCount].y - this._plotPoints[this.positionCount - 1].y, this._plotPoints[this.positionCount].x - this._plotPoints[this.positionCount - 1].x) - Math.PI;
      L154:
    }

    function draw(arg1:Event = null):void {
        this.positionCount = Math.floor(this.positionCount);
        var _local2 = Number(0);
        goto L44;
      L25:
        if (!(_local2 < this._plotPoints.length)) goto L44;
        _local2 = Number(_local2 + 2);
      L44:
        if (_local2 < this.positionCount) goto L25;
        if (this._follower == null) goto L67;
        this.dragFollower();
      L67:
        this.prevPosition = this.positionCount;
    }

    function dragFollower():void {
        if (!(this.positionCount < this._plotPoints.length)) goto L131;
        if (this.tweenPS) goto L71;
        this._follower.x = this._plotPoints[this.positionCount].x;
        this._follower.y = this._plotPoints[this.positionCount].y;
        goto L121;
      L71:
        (this._follower as ParticleSystem).emitterX = this._plotPoints[this.positionCount].x;
        (this._follower as ParticleSystem).emitterY = this._plotPoints[this.positionCount].y;
      L121:
        if (!(this.positionCount > 0)) goto L131;
      L131:
    }

    function set follower(arg1:DisplayObject):void {
        this._follower = arg1;
    }

    function plotPoint(arg1:Number):Object {
        var _local2 = {"x": 0, "y": 0};
        _local2.x = (1 - arg1) * (1 - arg1) * (1 - arg1) * this._points[0].x;
        _local2.y = (1 - arg1) * (1 - arg1) * (1 - arg1) * this._points[0].y;
        _local2.stroke = (1 - arg1) * (1 - arg1) * (1 - arg1) * 0.5;
        if (!(this._points.length > 1)) goto L206;
        _local2.x = _local2.x + 3 * ((1 - arg1) * (1 - arg1)) * arg1 * this._points[1].x;
        _local2.y = _local2.y + 3 * ((1 - arg1) * (1 - arg1)) * arg1 * this._points[1].y;
        _local2.stroke = 3 * ((1 - arg1) * (1 - arg1)) * arg1 * 5;
      L206:
        if (!(this._points.length > 2)) goto L303;
        _local2.x = _local2.x + 3 * (1 - arg1) * (arg1 * arg1) * this._points[2].x;
        _local2.y = _local2.y + 3 * (1 - arg1) * (arg1 * arg1) * this._points[2].y;
        _local2.stroke = 3 * (1 - arg1) * (arg1 * arg1) * 5;
      L303:
        if (!(this._points.length > 3)) goto L382;
        _local2.x = _local2.x + arg1 * arg1 * arg1 * this._points[3].x;
        _local2.y = _local2.y + arg1 * arg1 * arg1 * this._points[3].y;
        _local2.stroke = arg1 * arg1 * arg1 * 0.5;
      L382:
        if (!(this._points.length > 4)) goto L471;
        _local2.x = _local2.x + (1 - arg1) * arg1 * arg1 * arg1 * this._points[4].x;
        _local2.y = _local2.y + (1 - arg1) * arg1 * arg1 * arg1 * this._points[4].y;
        _local2.stroke = arg1 * arg1 * arg1 * 0.5;
      L471:
        return _local2;
    }
}
}
