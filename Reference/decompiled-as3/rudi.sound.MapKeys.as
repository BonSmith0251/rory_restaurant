package rudi.sound {
class MapKeys {
    var map:Map = null;

    function MapKeys():* {
        super();
    }

    function MapKeys_new(arg1:Map):MapKeys {
        this.map = arg1;
        return this;
    }

    function MapKeys_new2():MapKeys {
        return this;
    }

    function ObjectEnumerator():KeyEnumerator {
        return new KeyEnumerator().KeyEnumerator_new(this.map.FirstNode());
    }
}
}
