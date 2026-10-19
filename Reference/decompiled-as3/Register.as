package {
class Register {
    var _emitCode:uint;
    var _name:String;
    var _longName:String;
    var _flags:uint;
    var _range:uint;

    function Register(arg1:String, arg2:String, arg3:uint, arg4:uint, arg5:uint):* {
        super();
        this._name = arg1;
        this._longName = arg2;
        this._emitCode = arg3;
        this._range = arg4;
        this._flags = arg5;
    }

    function get emitCode():uint {
        return this._emitCode;
    }

    function get longName():String {
        return this._longName;
    }

    function get name():String {
        return this._name;
    }

    function get flags():uint {
        return this._flags;
    }

    function get range():uint {
        return this._range;
    }

    function toString():String {
        return "[Register name=\"" + this._name + "\", longName=\"" + this._longName + "\", emitCode=" + this._emitCode + ", range=" + this._range + ", flags=" + this._flags + "]";
    }
}
}
