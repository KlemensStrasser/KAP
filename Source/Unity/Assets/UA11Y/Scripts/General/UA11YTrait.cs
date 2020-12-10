
using System;

/// Indication on how the accessibilityElement should be treated. 
/// Based on https://developer.apple.com/documentation/uikit/accessibility/uiaccessibility/accessibility_traits?language=objc
public class UA11YTrait
{
    public UA11YTrait(string identifier)
    {
        this.identifier = identifier;
    }

    private string identifier { get; set; }

    // UI Element Types
    public static UA11YTrait Button { get { return new UA11YTrait("Button"); } }
    public static UA11YTrait Toggle { get { return new UA11YTrait("Toggle"); } }
    public static UA11YTrait Link { get { return new UA11YTrait("Link"); } }
    public static UA11YTrait Image { get { return new UA11YTrait("Image"); } }
    public static UA11YTrait StaticText { get { return new UA11YTrait("StaticText"); } }
    public static UA11YTrait Header { get { return new UA11YTrait("Header"); } }
    public static UA11YTrait SummaryElement { get { return new UA11YTrait("SummaryElement"); } }
    public static UA11YTrait Adjustable { get { return new UA11YTrait("Adjustable"); } }

    // Properties
    public static UA11YTrait Selected { get { return new UA11YTrait("Selected"); } }
    public static UA11YTrait NotEnabled { get { return new UA11YTrait("NotEnabled"); } }
    public static UA11YTrait AllowsDirectInteraction { get { return new UA11YTrait("AllowsDirectInteraction"); } }
    public static UA11YTrait HideFromScreenReader { get { return new UA11YTrait("HideFromScreenReader"); } }

    // TODO: Localize

    public string GetIdentifier()
    {
        return identifier;
    }

    override public string ToString()
    {
        string stringRepresentation;
        if (this.Equals(StaticText) || this.Equals(HideFromScreenReader))
        {
            stringRepresentation = "";
        }
        else
        {
            stringRepresentation = identifier;
        }

        return stringRepresentation;
    }

    public override bool Equals(object obj)
    {
        bool isEqual = false;
        if (obj.GetType() == this.GetType())
        {
            isEqual = Equals(obj as UA11YTrait);
        }

        return isEqual;
    }

    public bool Equals(UA11YTrait trait)
    {
        return this.identifier.Equals(trait.identifier);
    }

    public override int GetHashCode()
    {
        return identifier.GetHashCode();
    }
}