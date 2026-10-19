package {
class OpCode {
    var _emitCode:uint;
    var _flags:uint;
    var _name:String;
    var _numRegister:uint;

    function OpCode(arg1:String, arg2:uint, arg3:uint, arg4:uint):* {
        super();
        this._name = arg1;
        this._numRegister = arg2;
        this._emitCode = arg3;
        this._flags = arg4;
    }

    function get emitCode():uint {
        return this._emitCode;
    }

    function get flags():uint {
        return this._flags;
    }

    function get name():String {
        return this._name;
    }

    function get numRegister():uint {
        return this._numRegister;
    }

    function toString():String {
        return "[OpCode name=\"" + this._name + "\", numRegister=" + this._numRegister + ", emitCode=" + this._emitCode + ", flags=" + this._flags + "]";
    }
}
}
