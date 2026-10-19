package rudi.sound {
class Map {
    var root:Node = null;

    function Map():* {
        super();
    }

    function Map_new():Map {
        return this;
    }

    function Clear():int {
        this.root = null;
        return 0;
    }

    function Count():int {
        if (this.root == null) goto L22;
        return this.root.Count(0);
      L22:
        return 0;
    }

    function Compare(arg1:String, arg2:String):int {
        return 0;
    }

    function RotateLeft(arg1:Node):int {
        var _local2 = arg1.right;
        arg1.right = _local2.left;
        if (_local2.left == null) goto L34;
        _local2.left.parent = arg1;
      L34:
        _local2.parent = arg1.parent;
        if (arg1.parent == null) goto L87;
        if (arg1 != arg1.parent.left) goto L75;
        arg1.parent.left = _local2;
        goto L83;
      L75:
        arg1.parent.right = _local2;
      L83:
        goto L92;
      L87:
        this.root = _local2;
      L92:
        _local2.left = arg1;
        arg1.parent = _local2;
        return 0;
    }

    function RotateRight(arg1:Node):int {
        var _local2 = arg1.left;
        arg1.left = _local2.right;
        if (_local2.right == null) goto L34;
        _local2.right.parent = arg1;
      L34:
        _local2.parent = arg1.parent;
        if (arg1.parent == null) goto L87;
        if (arg1 != arg1.parent.right) goto L75;
        arg1.parent.right = _local2;
        goto L83;
      L75:
        arg1.parent.left = _local2;
      L83:
        goto L92;
      L87:
        this.root = _local2;
      L92:
        _local2.right = arg1;
        arg1.parent = _local2;
        return 0;
    }

    function InsertFixup(arg1:Node):int {
        var _local2 = null;
        var _local3 = null;
        goto L291;
      L14:
        if (arg1.parent != arg1.parent.parent.left) goto L164;
        _local2 = arg1.parent.parent.right;
        var $t0 = !(_local2 == null);
        $s0 = $t0;
        if (!$t0) goto L63;
        $s0 = _local2.color == -1;
      L63:
        if (!$s0) goto L102;
        arg1.parent.color = 1;
        _local2.color = 1;
        _local2.parent.color = -1;
        arg1 = _local2.parent;
        goto L160;
      L102:
        if (arg1 != arg1.parent.right) goto L127;
        arg1 = arg1.parent;
        this.RotateLeft(arg1);
      L127:
        arg1.parent.color = 1;
        arg1.parent.parent.color = -1;
        this.RotateRight(arg1.parent.parent);
      L160:
        goto L291;
      L164:
        _local3 = arg1.parent.parent.left;
        var $t1 = !(_local3 == null);
        $s0 = $t1;
        if (!$t1) goto L194;
        $s0 = _local3.color == -1;
      L194:
        if (!$s0) goto L233;
        arg1.parent.color = 1;
        _local3.color = 1;
        _local3.parent.color = -1;
        arg1 = _local3.parent;
        goto L291;
      L233:
        if (arg1 != arg1.parent.left) goto L258;
        arg1 = arg1.parent;
        this.RotateRight(arg1);
      L258:
        arg1.parent.color = 1;
        arg1.parent.parent.color = -1;
        this.RotateLeft(arg1.parent.parent);
      L291:
        var $t2 = !(arg1.parent == null);
        $s0 = $t2;
        if (!$t2) goto L314;
        $s0 = arg1.parent.color == -1;
      L314:
        if (!$s0) goto L330;
        $s0 = !(arg1.parent.parent == null);
      L330:
        if ($s0) goto L14;
        this.root.color = 1;
        return 0;
    }

    function Set(arg1:String, arg2:audio_Sound):Boolean {
        var _local3 = this.root;
        var _local4 = null;
        var _local5 = 0;
        goto L85;
      L22:
        _local4 = _local3;
        var $t0 = int(this.Compare(arg1, _local3.key));
        _local5 = $t0;
        if (!($t0 > 0)) goto L59;
        _local3 = _local3.right;
        goto L85;
      L59:
        if (!(_local5 < 0)) goto L78;
        _local3 = _local3.left;
        goto L85;
      L78:
        _local3.value = arg2;
        return false;
      L85:
        if (_local3 != null) goto L22;
        _local3 = new Node().Node_new(arg1, arg2, -1, _local4);
        if (_local4 == null) goto L152;
        if (!(_local5 > 0)) goto L136;
        _local4.right = _local3;
        goto L142;
      L136:
        _local4.left = _local3;
      L142:
        this.InsertFixup(_local3);
        goto L157;
      L152:
        this.root = _local3;
      L157:
        return true;
    }

    function FindNode(arg1:String):Node {
        var _local3 = 0;
        var _local2 = this.root;
        goto L67;
      L16:
        _local3 = int(this.Compare(arg1, _local2.key));
        if (!(_local3 > 0)) goto L47;
        _local2 = _local2.right;
        goto L67;
      L47:
        if (!(_local3 < 0)) goto L65;
        _local2 = _local2.left;
        goto L67;
      L65:
        return _local2;
      L67:
        if (_local2 != null) goto L16;
        return _local2;
    }

    function Get(arg1:String):audio_Sound {
        var _local2 = this.FindNode(arg1);
        if (_local2 == null) goto L22;
        return _local2.value;
      L22:
        return null;
    }

    function Contains(arg1:String):Boolean {
        return !(this.FindNode(arg1) == null);
    }

    function Keys():MapKeys {
        return new MapKeys().MapKeys_new(this);
    }

    function FirstNode():Node {
        if (!(this.root == null)) goto L15;
        return null;
      L15:
        var _local1 = this.root;
        goto L34;
      L26:
        _local1 = _local1.left;
      L34:
        if (_local1.left != null) goto L26;
        return _local1;
    }

    function RemoveNode(arg1:Node):int {
        var _local2 = null;
        var _local3 = null;
        if (!(arg1.left == null)) goto L36;
        _local2 = arg1;
        _local3 = arg1.right;
        goto L113;
      L36:
        if (!(arg1.right == null)) goto L62;
        _local2 = arg1;
        _local3 = arg1.left;
        goto L113;
      L62:
        _local2 = arg1.left;
        goto L81;
      L73:
        _local2 = _local2.right;
      L81:
        if (_local2.right != null) goto L73;
        _local3 = _local2.left;
        arg1.key = _local2.key;
        arg1.value = _local2.value;
      L113:
        var _local4 = _local2.parent;
        if (_local3 == null) goto L133;
        _local3.parent = _local4;
      L133:
        if (!(_local4 == null)) goto L150;
        this.root = _local3;
        return 0;
      L150:
        if (_local2 != _local4.left) goto L170;
        _local4.left = _local3;
        goto L176;
      L170:
        _local4.right = _local3;
      L176:
        if (_local2.color != 1) goto L194;
        this.DeleteFixup(_local3, _local4);
      L194:
        return 0;
    }

    function Remove(arg1:String):int {
        var _local2 = this.FindNode(arg1);
        if (!(_local2 == null)) goto L22;
        return 0;
      L22:
        this.RemoveNode(_local2);
        return 1;
    }

    function DeleteFixup(arg1:Node, arg2:Node):int {
        var _local3 = null;
        var _local4 = null;
        goto L471;
      L15:
        if (arg1 != arg2.left) goto L242;
        _local3 = arg2.right;
        if (_local3.color != -1) goto L67;
        _local3.color = 1;
        arg2.color = -1;
        this.RotateLeft(arg2);
        _local3 = arg2.right;
      L67:
        var $t0 = !!(_local3.left == null);
        $s0 = $t0;
        if ($t0) goto L91;
        $s0 = _local3.left.color == 1;
      L91:
        if (!$s0) goto L121;
        var $t1 = !!(_local3.right == null);
        $s0 = $t1;
        if ($t1) goto L121;
        $s0 = _local3.right.color == 1;
      L121:
        if (!$s0) goto L146;
        _local3.color = -1;
        arg1 = arg2;
        arg2 = arg2.parent;
        goto L238;
      L146:
        var $t2 = !!(_local3.right == null);
        $s0 = $t2;
        if ($t2) goto L170;
        $s0 = _local3.right.color == 1;
      L170:
        if (!$s0) goto L202;
        _local3.left.color = 1;
        _local3.color = -1;
        this.RotateRight(_local3);
        _local3 = arg2.right;
      L202:
        _local3.color = arg2.color;
        arg2.color = 1;
        _local3.right.color = 1;
        this.RotateLeft(arg2);
        arg1 = this.root;
      L238:
        goto L471;
      L242:
        var $t3 = arg2.left;
        _local4 = $t3;
        if ($t3.color != -1) goto L287;
        _local4.color = 1;
        arg2.color = -1;
        this.RotateRight(arg2);
        _local4 = arg2.left;
      L287:
        var $t4 = !!(_local4.right == null);
        $s0 = $t4;
        if ($t4) goto L313;
        $s0 = _local4.right.color == 1;
      L313:
        if (!$s0) goto L345;
        var $t5 = !!(_local4.left == null);
        $s0 = $t5;
        if ($t5) goto L345;
        $s0 = _local4.left.color == 1;
      L345:
        if (!$s0) goto L371;
        _local4.color = -1;
        arg1 = arg2;
        arg2 = arg2.parent;
        goto L471;
      L371:
        var $t6 = !!(_local4.left == null);
        $s0 = $t6;
        if ($t6) goto L397;
        $s0 = _local4.left.color == 1;
      L397:
        if (!$s0) goto L433;
        _local4.right.color = 1;
        _local4.color = -1;
        this.RotateLeft(_local4);
        _local4 = arg2.left;
      L433:
        _local4.color = arg2.color;
        arg2.color = 1;
        _local4.left.color = 1;
        this.RotateRight(arg2);
        arg1 = this.root;
      L471:
        var $t7 = !(arg1 == this.root);
        $s0 = $t7;
        if (!$t7) goto L502;
        var $t8 = !!(arg1 == null);
        $s0 = $t8;
        if ($t8) goto L502;
        $s0 = arg1.color == 1;
      L502:
        if ($s0) goto L15;
        if (arg1 == null) goto L518;
        arg1.color = 1;
      L518:
        return 0;
    }
}
}
