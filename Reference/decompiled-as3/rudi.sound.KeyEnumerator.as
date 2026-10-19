package rudi.sound {
class KeyEnumerator {
    var node:Node = null;

    function KeyEnumerator():* {
        super();
    }

    function KeyEnumerator_new(arg1:Node):KeyEnumerator {
        this.node = arg1;
        return this;
    }

    function KeyEnumerator_new2():KeyEnumerator {
        return this;
    }

    function HasNext():Boolean {
        return !(this.node == null);
    }

    function NextObject():String {
        var _local1 = this.node;
        this.node = this.node.NextNode();
        return _local1.key;
    }
}
}
