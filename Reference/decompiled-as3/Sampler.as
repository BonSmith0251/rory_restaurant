package {
class Sampler {
    var _flag:uint;
    var _mask:uint;
    var _name:String;

    function Sampler(arg1:String, arg2:uint, arg3:uint):* {
        super();
        this._name = arg1;
        this._flag = arg2;
        this._mask = arg3;
    }

    function get flag():uint {
        return this._flag;
    }

    function get mask():uint {
        return this._mask;
    }

    function get name():String {
        return this._name;
    }

    function toString():String {
        return "[Sampler name=\"" + this._name + "\", flag=\"" + this._flag + "\", mask=" + this.mask + "]";
    }
}
}
