
using System;

/// Indication on how the accessibilityElement should be treated. 
/// Based on https://developer.apple.com/documentation/uikit/accessibility/uiaccessibility/accessibility_traits?language=objc
public class UA11YTrait
{
    private UA11YTrait(ulong value) { Value = value; }

    public ulong Value { get; set; }

    // TODO: Support for those should definitley be implemented at some point.
    public static UA11YTrait None         { get { return new UA11YTrait(0x0000000000000000); } }
    public static UA11YTrait Button       { get { return new UA11YTrait(0x0000000000000001); } }
    public static UA11YTrait Toggle       { get { return new UA11YTrait(0x0020000000000001); } }
    public static UA11YTrait Link         { get { return new UA11YTrait(0x0000000000000002); } }
    public static UA11YTrait Image        { get { return new UA11YTrait(0x0000000000000004); } }
    public static UA11YTrait Selected     { get { return new UA11YTrait(0x0000000000000008); } }
    public static UA11YTrait StaticText   { get { return new UA11YTrait(0x0000000000000040); } }
    public static UA11YTrait Header       { get { return new UA11YTrait(0x0000000000010000); } }
    public static UA11YTrait SummaryElement { get { return new UA11YTrait(0x0000000000000080); } }
    public static UA11YTrait NotEnabled   { get { return new UA11YTrait(0x0000000000000100); } }
    public static UA11YTrait UpdatesFrequently { get { return new UA11YTrait(0x0000000000000200); } }
    public static UA11YTrait Adjustable { get { return new UA11YTrait(0x0000000000001000); } }
    public static UA11YTrait AllowsDirectInteraction { get { return new UA11YTrait(0x0000000000002000); } }

    // TODO: Those I'm not sure about
    public static UA11YTrait PlaysSound   { get { return new UA11YTrait(0x0000000000000010); } }
    public static UA11YTrait KeyboardKey  { get { return new UA11YTrait(0x0000000000000020); } }
    public static UA11YTrait SearchField { get { return new UA11YTrait(0x0000000000000400); } }
    public static UA11YTrait StartsMediaSession { get { return new UA11YTrait(0x0000000000000800); } }
    public static UA11YTrait CausesPageTurn { get { return new UA11YTrait(0x0000000000004000); } }

    // TODO: This is just temporary. A element can have mutliple traits that are used in different parts of the description. Therefore, having a single ToString value makes no sense
    override public string ToString()
    {
        // TODO: Localize
        if (this.Value == UA11YTrait.Toggle.Value)
        {
            return "Toggle";
        }
        else if (this.Value == UA11YTrait.Button.Value)
        {
            return "Button";
        }
        else if (this.Value == UA11YTrait.Image.Value)
        {
            return "Image";
        }

        return "";
    }
}