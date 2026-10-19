package rudi.sound {
class StringMap extends Map {

    function StringMap():* {
        super();
    }

    function StringMap_new():StringMap {
        super.Map_new();
        return this;
    }

    override function Compare(arg1:String, arg2:String):int {
        return this.string_compare(arg1, arg2);
    }

    function string_compare(arg1:String, arg2:String):int {
        var _local4 = 0;
        var _local5 = 0;
        var _local3 = int(Math.min(arg1.length, arg2.length));
        _local4 = 0;
        goto L64;
      L35:
        var $t0 = int(arg1.charCodeAt(_local4) - arg2.charCodeAt(_local4));
        _local5 = $t0;
        if (!$t0) goto L62;
        return _local5;
      L62:
        _local4++;
      L64:
        if (_local4 < _local3) goto L35;
        return arg1.length - arg2.length;
    }
}
}
