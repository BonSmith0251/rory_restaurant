package {
class Match3Object extends Image {
    var idx:int;
    var aktif:Boolean;
    var used:Boolean;
    var type:int;
    var col:int;
    var row:int;
    var vy:Number = 0;
    var removed:Boolean;
    var isDropping:Boolean;
    var isSwapping:Boolean;
    var double:Boolean;
    var onTween:Boolean;
    var asMatch3Part:Boolean;
    var asMatch4Part:Boolean;
    var asMatch5Part:Boolean;
    var asMatchMorePart:Boolean;
    var asBomb:Boolean;
    var asVertical:Boolean;
    var asHorizontal:Boolean;
    var asColor:Boolean;
    var asSpecial:Boolean;
    var toBombTile:Boolean;
    var toColorTile:Boolean;
    var specialPossible:Boolean;
    var powerActivated:Boolean;
    var frozen:Boolean;
    var frozenTileIdx:int;
    var hover:Boolean;

    function Match3Object(arg1:Texture):* {
        super(arg1);
    }
}
}
