
using System;

/// Indication on how the accessibilityElement should be treated. 
/// Based on https://developer.apple.com/documentation/uikit/accessibility/uiaccessibility/accessibility_traits?language=objc
public class KAPTrait
{
    private KAPTrait(ulong value) { Value = value; }

    public ulong Value { get; set; }

    // TODO: Support for those should definitley be implemented at some point.
    public static KAPTrait None         { get { return new KAPTrait(0x0000000000000000); } }
    public static KAPTrait Button       { get { return new KAPTrait(0x0000000000000001); } }
    public static KAPTrait Toggle       { get { return new KAPTrait(0x0020000000000001); } }
    public static KAPTrait Link         { get { return new KAPTrait(0x0000000000000002); } }
    public static KAPTrait Image        { get { return new KAPTrait(0x0000000000000004); } }
    public static KAPTrait Selected     { get { return new KAPTrait(0x0000000000000008); } }
    public static KAPTrait StaticText   { get { return new KAPTrait(0x0000000000000040); } }
    public static KAPTrait Header       { get { return new KAPTrait(0x0000000000010000); } }
    public static KAPTrait SummaryElement { get { return new KAPTrait(0x0000000000000080); } }
    public static KAPTrait NotEnabled   { get { return new KAPTrait(0x0000000000000100); } }
    public static KAPTrait UpdatesFrequently { get { return new KAPTrait(0x0000000000000200); } }
    public static KAPTrait Adjustable { get { return new KAPTrait(0x0000000000001000); } }
    public static KAPTrait AllowsDirectInteraction { get { return new KAPTrait(0x0000000000002000); } }

    // TODO: Those I'm not sure about
    public static KAPTrait PlaysSound   { get { return new KAPTrait(0x0000000000000010); } }
    public static KAPTrait KeyboardKey  { get { return new KAPTrait(0x0000000000000020); } }
    public static KAPTrait SearchField { get { return new KAPTrait(0x0000000000000400); } }
    public static KAPTrait StartsMediaSession { get { return new KAPTrait(0x0000000000000800); } }
    public static KAPTrait CausesPageTurn { get { return new KAPTrait(0x0000000000004000); } }

    // TODO: This is just temporary. A element can have mutliple traits that are used in different parts of the description. Therefore, having a single ToString value makes no sense
    override public string ToString()
    {
        // TODO: Localize
        if (this.Value == KAPTrait.Toggle.Value)
        {
            return "Toggle";
        }
        else if (this.Value == KAPTrait.Button.Value)
        {
            return "Button";
        }

        return "";
    }
}