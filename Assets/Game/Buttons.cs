// Ported from rudi.cSprite, ButtonWithSound, TNonStateButton, TStringButton, ExtendTextField.
using System;
using Starlite;

public class cSprite : Image
{
    public bool aktif;
    public int idx;
    public bool onCenter;
    public int itemIdx;
    public int type;
    public string ingredientName;
    public float _paddingX = 0;
    public float _paddingY = 0;
    public bool hover;
    public float xDif;
    public float yDif;

    public cSprite(Texture arg1) : base(arg1) { }

    public float paddingX { get => _paddingX; set => _paddingX = value; }
    public float paddingY { get => _paddingY; set => _paddingY = value; }

    public override DisplayObject hitTest(Point arg1, bool arg2 = false)
    {
        if (arg2 && (!visible || !touchable)) return null;
        var b = getBounds(this);
        b.inflate(_paddingX, _paddingY);
        return b.containsPoint(arg1) ? this : null;
    }
}

public class ExtendTextField : TextField
{
    public bool aktif;
    public int idx;
    public bool itemIsFound;
    public int placeNumber;

    public ExtendTextField(float arg1, float arg2, string arg3, string arg4 = "Verdana", float arg5 = 12, uint arg6 = 0, bool arg7 = false)
        : base(arg1, arg2, arg3, arg4, arg5, arg6, arg7) { }
}

public class TStringButton : ExtendTextField
{
    public bool hover;

    public TStringButton(float arg1, float arg2, string arg3, string arg4 = "Verdana", float arg5 = 12, uint arg6 = 0, bool arg7 = false)
        : base(arg1, arg2, arg3, arg4, arg5, arg6, arg7)
    {
        addEventListener(TouchEvent.TOUCH, touchText);
        useHandCursor = true;
    }

    public void touchText(Event arg1)
    {
        var e = (TouchEvent)arg1;
        var btn = e.currentTarget as TStringButton;
        var t = e.getTouch(btn, "hover");
        if (t != null)
        {
            if (!btn.hover)
            {
                btn.scaleX = btn.scaleX * 1.1f;
                btn.scaleY = btn.scaleY * 1.1f;
                btn.hover = true;
            }
        }
        else if (btn.hover)
        {
            btn.scaleX = btn.scaleX * (10f / 11f);
            btn.scaleY = btn.scaleY * (10f / 11f);
            btn.hover = false;
        }
    }
}

public class ButtonWithSound : DisplayObjectContainer
{
    public const float MAX_DRAG_DIST = 50;
    public Global _global;
    public Texture mUpState;
    public Texture mDownState;
    public Sprite mContents;
    public Image mBackground;
    public TextField mTextField;
    public Rectangle mTextBounds;
    public float mScaleWhenDown;
    public float mAlphaWhenDisabled;
    public bool mEnabled;
    public bool mIsDown;
    public bool mUseHandCursor;

    public ButtonWithSound(Texture arg1, string arg2 = "", Texture arg3 = null)
    {
        _global = Global.getInstance();
        if (arg1 == null) throw new ArgumentException("Texture cannot be null");
        mUpState = arg1;
        mDownState = arg3 != null ? arg3 : arg1;
        mBackground = new Image(arg1);
        mScaleWhenDown = arg3 != null ? 1 : 0.9f;
        mAlphaWhenDisabled = 0.5f;
        mEnabled = true;
        mIsDown = false;
        mUseHandCursor = true;
        mTextBounds = new Rectangle(0, 0, arg1.width, arg1.height);
        mContents = new Sprite();
        mContents.addChild(mBackground);
        addChild(mContents);
        addEventListener(TouchEvent.TOUCH, onTouch);
        text = arg2;
    }

    public void resetContents()
    {
        mIsDown = false;
        mBackground.texture = mUpState;
        mContents.x = mContents.y = 0;
        mContents.scaleX = mContents.scaleY = 1;
    }

    public void createTextField()
    {
        if (mTextField == null)
        {
            mTextField = new TextField(mTextBounds.width, mTextBounds.height, "");
            mTextField.vAlign = VAlign.CENTER;
            mTextField.hAlign = HAlign.CENTER;
            mTextField.touchable = false;
            mTextField.autoScale = true;
        }
        mTextField.width = mTextBounds.width;
        mTextField.height = mTextBounds.height;
        mTextField.x = mTextBounds.x;
        mTextField.y = mTextBounds.y;
    }

    public void onTouch(Event arg1)
    {
        var e = (TouchEvent)arg1;
        Mouse.cursor = mUseHandCursor && mEnabled && e.interactsWith(this) ? MouseCursor.BUTTON : MouseCursor.AUTO;
        var touch = e.getTouch(this);
        if (!mEnabled || touch == null) return;
        if (touch.phase == TouchPhase.BEGAN && !mIsDown)
        {
            mBackground.texture = mDownState;
            mContents.scaleX = mContents.scaleY = mScaleWhenDown;
            mContents.x = (1 - mScaleWhenDown) / 2 * mBackground.width;
            mContents.y = (1 - mScaleWhenDown) / 2 * mBackground.height;
            mIsDown = true;
        }
        else if (touch.phase == TouchPhase.MOVED && mIsDown)
        {
            var b = getBounds(stage);
            if (touch.globalX < b.x - MAX_DRAG_DIST || touch.globalY < b.y - MAX_DRAG_DIST ||
                touch.globalX > b.x + b.width + MAX_DRAG_DIST || touch.globalY > b.y + b.height + MAX_DRAG_DIST)
                resetContents();
        }
        else if (touch.phase == TouchPhase.ENDED && mIsDown)
        {
            _global._soundAsset.playSFX("click_v1");
            resetContents();
            dispatchEventWith(Event.TRIGGERED, true);
        }
    }

    public float scaleWhenDown { get => mScaleWhenDown; set => mScaleWhenDown = value; }
    public float alphaWhenDisabled { get => mAlphaWhenDisabled; set => mAlphaWhenDisabled = value; }

    public virtual bool enabled
    {
        get => mEnabled;
        set
        {
            if (mEnabled == value) return;
            mEnabled = value;
            mContents.alpha = value ? 1 : mAlphaWhenDisabled;
            resetContents();
        }
    }

    public string text
    {
        get => mTextField != null ? mTextField.text : "";
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                if (mTextField != null)
                {
                    mTextField.text = value ?? "";
                    mTextField.removeFromParent();
                }
            }
            else
            {
                createTextField();
                mTextField.text = value;
                if (mTextField.parent == null) mContents.addChild(mTextField);
            }
        }
    }

    public string fontName { get => mTextField != null ? mTextField.fontName : "Verdana"; set { createTextField(); mTextField.fontName = value; } }
    public float fontSize { get => mTextField != null ? mTextField.fontSize : 12; set { createTextField(); mTextField.fontSize = value; } }
    public uint fontColor { get => mTextField != null ? mTextField.color : 0; set { createTextField(); mTextField.color = value; } }
    public bool fontBold { get => mTextField != null && mTextField.bold; set { createTextField(); mTextField.bold = value; } }

    public Texture upState
    {
        get => mUpState;
        set
        {
            if (mUpState == value) return;
            mUpState = value;
            if (!mIsDown) mBackground.texture = value;
        }
    }

    public Texture downState
    {
        get => mDownState;
        set
        {
            if (mDownState == value) return;
            mDownState = value;
            if (mIsDown) mBackground.texture = value;
        }
    }

    public string textVAlign { get => mTextField.vAlign; set { createTextField(); mTextField.vAlign = value; } }
    public string textHAlign { get => mTextField.hAlign; set { createTextField(); mTextField.hAlign = value; } }
    public Rectangle textBounds { get => mTextBounds.clone(); set { mTextBounds = value.clone(); createTextField(); } }
    public override bool useHandCursor { get => mUseHandCursor; set => mUseHandCursor = value; }
}

public class TNonStateButton : ButtonWithSound
{
    public const int CONFIRM_EXIT = 1;
    public const int CANCEL_BUTTON = 2;
    public const int MOREGAMES_BUTTON = 3;
    public const int NEWGAME_BUTTON = 4;
    public const int CONTINUEGAME_BUTTON = 5;
    public const int CONFIRM_BUTTON = 6;
    public const int SUBMIT_BUTTON = 7;
    public const int LEADERBOARD_BUTTON = 8;
    public const int REPLAY_BUTTON = 9;
    public const int MAINMENU_BUTTON = 10;
    public const int LEFT_ARROW = 11;
    public const int RIGHT_ARROW = 12;
    public const int PLAY_BUTTON = 13;
    public const int EXTRAS_BUTTON = 14;
    public const int NEXT_LEVEL = 15;
    public const int TRY_AGAIN = 16;
    public const int YES_BUTTON = 17;
    public const int NO_BUTTON = 18;
    public const int OK_BUTTON = 19;
    public const int HELP_BUTTON = 20;
    public const int DELETE_BUTTON = 21;
    public const int CREATE_BUTOTN = 22;
    public const int SELECT_BUTTON = 23;
    public const int DONE_CREATE_PLAYER = 24;
    public const int SOUND_BUTTON = 25;
    public const int MUSIC_BUTTON = 26;
    public const int RESUME_BUTTON = 27;
    public const int KITCHEN = 28;
    public const int KITCHEN_WITH_CONFIRMATION = 29;
    public const int NEXT_WINDOW = 30;
    public const int ENDING = 31;
    public const int HO_BUTTON = 32;
    public const int DM_BUTTON = 33;
    public const int UNLOCK_BUTTON = 34;
    public const int FULLSCREEN_BUTTON = 35;
    public ExtendTextField _text;
    public cSprite _image;
    public string textureName;
    public int idx;
    public bool hover;
    public bool _enabled;
    public bool initPosX;
    public bool initPosY;
    public bool isPivotSetX;
    public bool isPivotSetY;

    public TNonStateButton(Texture arg1, string arg2 = "", string arg3 = "", int arg4 = 20, uint arg5 = 0) : base(arg1, arg2)
    {
        fontName = arg3;
        fontSize = arg4;
        fontColor = arg5;
        _image = new cSprite(arg1);
        addChild(_image);
        if (text == "Test") text = "";
        _text = new ExtendTextField(_image.width, _image.height, arg2, arg3, arg4, arg5);
        _text.y = 0;
        _text.vAlign = "center";
        _text.hAlign = "center";
        addChild(_text);
        addEventListener(TouchEvent.TOUCH, touchButton);
    }

    public void touchButton(Event arg1)
    {
        var e = (TouchEvent)arg1;
        var btn = e.currentTarget as TNonStateButton;
        var t = e.getTouch(btn, "hover");
        bool arrow = btn._image.name == "" + Kitchen.RIGHT_BUTTON || btn._image.name == "" + Kitchen.LEFT_BUTTON;
        if (t != null)
        {
            if (!btn.hover)
            {
                float f = arrow ? 1.2f : 1.05f;
                btn.scaleX = btn.scaleX * f;
                btn.scaleY = btn.scaleY * f;
                btn.hover = true;
            }
        }
        else if (btn.hover)
        {
            float f = arrow ? 10f / 12f : 10f / 10.5f;
            btn.scaleX = btn.scaleX * f;
            btn.scaleY = btn.scaleY * f;
            btn.hover = false;
        }
    }

    // On first placement the pivot is moved to the horizontal/vertical center (unless already set),
    // and later positions keep compensating for it, exactly like the AS3 overrides.
    public override float x
    {
        get => base.x;
        set
        {
            if (!initPosX)
            {
                if (pivotX == 0)
                {
                    pivotX = width / 2;
                    base.x = value + pivotX;
                    isPivotSetX = false;
                }
                else
                {
                    isPivotSetX = true;
                    base.x = value;
                }
                initPosX = true;
            }
            else if (!isPivotSetX) base.x = value + pivotX;
            else base.x = value;
        }
    }

    public override float y
    {
        get => base.y;
        set
        {
            if (!initPosY)
            {
                if (pivotY == 0)
                {
                    pivotY = height / 2;
                    base.y = value + pivotY;
                    isPivotSetY = false;
                }
                else
                {
                    isPivotSetY = true;
                    base.y = value;
                }
                initPosY = true;
            }
            else if (!isPivotSetY) base.y = value + pivotY;
            else base.y = value;
        }
    }

    public override void dispose()
    {
        base.dispose();
        if (_image != null) _image.removeFromParent(true);
        _image = null;
        if (_text != null) _text.removeFromParent(true);
        _text = null;
    }

    public override bool enabled
    {
        get => _enabled;
        set
        {
            if (value)
            {
                alpha = 1;
                touchable = true;
            }
            else
            {
                alpha = 0.5f;
                touchable = false;
            }
            _enabled = value;
        }
    }
}
