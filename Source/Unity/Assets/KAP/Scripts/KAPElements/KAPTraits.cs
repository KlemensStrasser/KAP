/// Indication on how the accessibilityElement should be treated
public class KAPTrait
{
    private KAPTrait(string value) { Value = value; }

    public string Value { get; set; }

    // TODO: Localization
    public static KAPTrait Undefined { get { return new KAPTrait(null); } }
    public static KAPTrait None { get { return new KAPTrait(""); } }
    public static KAPTrait Button { get { return new KAPTrait("Button"); } }
    public static KAPTrait Text { get { return new KAPTrait(""); } }
    public static KAPTrait Title { get { return new KAPTrait(""); } }
}
