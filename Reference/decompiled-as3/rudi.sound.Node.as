package rudi.sound {
class Node {
    var left:Node = null;
    var right:Node = null;
    var key:String = "";
    var value:audio_Sound = null;
    var color:int = 0;
    var parent:Node = null;

    function Node():* {
        super();
    }

    function Count(arg1:int):int {
        if (this.left == null) goto L22;
        arg1 = int(this.left.Count(arg1));
      L22:
        if (this.right == null) goto L42;
        arg1 = int(this.right.Count(arg1));
      L42:
        return arg1 + 1;
    }

    function Node_new(arg1:String, arg2:audio_Sound, arg3:int, arg4:Node):Node {
        this.key = arg1;
        this.value = arg2;
        this.color = arg3;
        this.parent = arg4;
        return this;
    }

    function Node_new2():Node {
        return this;
    }

    function NextNode():Node {
        var _local1 = null;
        if (this.right == null) goto L45;
        _local1 = this.right;
        goto L34;
      L26:
        _local1 = _local1.left;
      L34:
        if (_local1.left != null) goto L26;
        return _local1;
      L45:
        _local1 = this;
        var _local2 = this.parent;
        goto L72;
      L60:
        _local1 = _local2;
        _local2 = _local2.parent;
      L72:
        var $t0 = !(_local2 == null);
        $s0 = $t0;
        if (!$t0) goto L88;
        $s0 = _local1 == _local2.right;
      L88:
        if ($s0) goto L60;
        return _local2;
    }
}
}
