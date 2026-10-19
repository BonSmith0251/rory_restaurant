package rudi {
class maxCrypto {
    static var _instance:maxCrypto;
    var key:Array;
    var separator:String = ",";

    function maxCrypto(arg1:PrivateClass):* {
        this.key = ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j"];
        super();
    }

    static function getInstance():maxCrypto {
        if (maxCrypto._instance != null) goto L29;
        maxCrypto._instance = new this(new PrivateClass());
      L29:
        return maxCrypto._instance;
    }

    function loadKey(arg1:Array):void {
        var _local2 = 0;
        _local2 = 0;
        goto L28;
      L12:
        this.key[_local2] = arg1[_local2];
        _local2++;
      L28:
        if (_local2 < 10) goto L12;
    }

    function changeKey(arg1:int, arg2:String):void {
        var $t0 = arg1 < 10;
        $s0 = $t0;
        if (!$t0) goto L16;
        $s0 = arg1 > -1;
      L16:
        if (!$s0) goto L29;
        this.key[arg1] = arg2;
      L29:
    }

    function STI(arg1:String):int {
        var _local3 = 0;
        var _local4 = 0;
        var _local2 = 0;
        var _local5 = int(arg1.length);
        _local3 = 0;
        goto L83;
      L26:
        _local4 = 0;
        goto L73;
      L35:
        if (arg1.charAt(_local3) != this.key[_local4]) goto L71;
        _local2 = int(_local2 * 10);
        _local2 = int(_local2 + _local4);
        goto L81;
      L71:
        _local4++;
      L73:
        if (_local4 < 10) goto L35;
      L81:
        _local3++;
      L83:
        if (_local3 < _local5) goto L26;
        return _local2;
    }

    function ITS(arg1:int):String {
        var _local3 = 0;
        var _local4 = 0;
        var _local2 = "";
        if (arg1 != 0) goto L23;
        return "a";
      L23:
        goto L49;
      L27:
        _local2 = _local2 + this.key[arg1 % 10];
        arg1 = int(arg1 / 10);
      L49:
        if (arg1 > 0) goto L27;
        return _local2.split("").reverse().join("");
    }

    function convertToDigit(arg1:String, arg2:int, arg3:int):String {
        var _local4 = null;
        var _local10 = 0;
        if (arg3 != 1) goto L21;
        _local4 = arg1;
      L21:
        if (arg3 != 2) goto L35;
        _local4 = "" + arg2;
      L35:
        var _local5 = int(_local4.length);
        var _local6 = [];
        var _local7 = 0;
        var _local8 = "";
        _local7 = int(Math.floor(_local5 / 3));
        var _local9 = 0;
        var $t0 = int(_local5 - _local7 * 3);
        _local9 = $t0;
        var $t1 = $t0 > 0;
        $s0 = $t1;
        if ($t1) goto L102;
        $s0 = _local5 > 1;
      L102:
        if (!$s0) goto L108;
        _local7++;
      L108:
        _local10 = 0;
        goto L269;
      L116:
        var $t2 = _local9 > 0;
        $s0 = $t2;
        if ($t2) goto L133;
        $s0 = _local5 > 1;
      L133:
        if (!$s0) goto L144;
        $s0 = _local10 == 0;
      L144:
        if (!$s0) goto L168;
        _local8 = "" + _local4.slice(0, _local9);
        goto L267;
      L168:
        if (_local8 != "") goto L221;
        _local8 = _local8 + _local4.slice(_local9 + _local10 + (_local10 - 1) * 2 - 1, _local9 + _local10 + (_local10 - 1) * 2 + 3 - 1);
        goto L267;
      L221:
        _local8 = _local8 + this.separator + _local4.slice(_local9 + _local10 + (_local10 - 1) * 2 - 1, _local9 + _local10 + (_local10 - 1) * 2 + 3 - 1);
      L267:
        _local10++;
      L269:
        if (_local10 < _local7) goto L116;
        return _local8;
    }

    function RandomRange(arg1:int, arg2:int):int {
        return Math.floor(Math.random() * (1 + arg2 - arg1)) + arg1;
    }

    function toMinute(arg1:int):String {
        var _local2 = int(arg1);
        var _local3 = 60;
        var _local4 = 1;
        var _local5 = 0;
        if (!(_local2 >= _local3)) goto L43;
        _local5 = int(Math.floor(_local2 / _local3));
        _local2 = int(_local2 - _local5 * _local3);
      L43:
        var _local6 = 0;
        if (!(_local2 >= _local4)) goto L77;
        _local6 = int(Math.floor(_local2 / _local4));
        _local2 = int(_local2 - _local6 * _local4);
      L77:
        var _local7 = "" + _local5;
        var _local8 = "" + _local6;
        if (_local7.length != 1) goto L113;
        _local7 = "0" + _local7;
      L113:
        if (_local8.length != 1) goto L133;
        _local8 = "0" + _local8;
      L133:
        return _local7 + ":" + _local8;
    }

    function toHour(arg1:int):String {
        var _local2 = int(arg1);
        var _local3 = int(60 * 60);
        var _local4 = 60;
        var _local5 = 1;
        var _local6 = 0;
        if (!(_local2 >= _local3)) goto L50;
        _local6 = int(Math.floor(_local2 / _local3));
        _local2 = int(_local2 - _local6 * _local3);
      L50:
        var _local7 = 0;
        if (!(_local2 >= _local4)) goto L84;
        _local7 = int(Math.floor(_local2 / _local4));
        _local2 = int(_local2 - _local7 * _local4);
      L84:
        var _local8 = 0;
        if (!(_local2 >= _local5)) goto L118;
        _local8 = int(Math.floor(_local2 / _local5));
        _local2 = int(_local2 - _local8 * _local5);
      L118:
        var _local9 = "" + _local6;
        var _local10 = "" + _local7;
        var _local11 = "" + _local8;
        if (_local9.length != 1) goto L162;
        _local9 = "0" + _local9;
      L162:
        if (_local10.length != 1) goto L182;
        _local10 = "0" + _local10;
      L182:
        if (_local11.length != 1) goto L202;
        _local11 = "0" + _local11;
      L202:
        return _local9 + ":" + _local10 + ":" + _local11;
    }

    function StripExt(arg1:String):String {
        var _local2 = int(arg1.lastIndexOf("."));
        var $t0 = !(_local2 == -1);
        $s0 = $t0;
        if (!$t0) goto L38;
        $s0 = arg1.indexOf("/", _local2 + 1) == -1;
      L38:
        if (!$s0) goto L59;
        $s0 = arg1.indexOf("\\", _local2 + 1) == -1;
      L59:
        if (!$s0) goto L72;
        return arg1.slice(0, _local2);
      L72:
        return arg1;
    }

    function StripDir(arg1:String):String {
        var _local2 = int(arg1.lastIndexOf("/"));
        if (_local2 != -1) goto L29;
        _local2 = int(arg1.lastIndexOf("\\"));
      L29:
        if (_local2 == -1) goto L46;
        return arg1.slice(_local2 + 1);
      L46:
        return arg1;
    }

    function StripAll(arg1:String):String {
        return this.StripDir(this.StripExt(arg1));
    }

    function ExtractDir(arg1:String):String {
        var _local2 = int(arg1.lastIndexOf("/"));
        if (_local2 != -1) goto L29;
        _local2 = int(arg1.lastIndexOf("\\"));
      L29:
        if (_local2 == -1) goto L45;
        return arg1.slice(0, _local2);
      L45:
        return "";
    }

    function ExtractExt(arg1:String):String {
        var _local2 = int(arg1.lastIndexOf("."));
        var $t0 = !(_local2 == -1);
        $s0 = $t0;
        if (!$t0) goto L38;
        $s0 = arg1.indexOf("/", _local2 + 1) == -1;
      L38:
        if (!$s0) goto L59;
        $s0 = arg1.indexOf("\\", _local2 + 1) == -1;
      L59:
        if (!$s0) goto L73;
        return arg1.slice(_local2 + 1);
      L73:
        return "";
    }
}
}
