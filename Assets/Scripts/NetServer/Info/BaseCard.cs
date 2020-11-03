
/// <summary>
/// 牌色
/// </summary>
public enum CardColors
{
    /// <summary>
    /// 黑色
    /// </summary>
    BLACK,
    /// <summary>
    /// 白色
    /// </summary>
    WHITE
}

/// <summary>
/// 卡牌的权值
/// </summary>
public enum CardWeight
{
    Zero,
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Eleven,
    /// <summary>
    /// 短线
    /// </summary>
    Line
}

/// <summary>
/// 卡牌的归属
/// </summary>
public enum CardBelongTo
{
    /// <summary>
    /// 归属牌库
    /// </summary>
    ZERO,
    /// <summary>
    /// 归属第一个成员
    /// </summary>
    FIRST,
    SECOND,
    THIRD,
    FOURTH,
}

/// <summary>
/// 卡牌的显隐
/// </summary>
public enum CardDisplay
{
    /// <summary>
    /// 显示（正面）
    /// </summary>
    True,
    /// <summary>
    /// 隐藏（背面）
    /// </summary>
    False
}



/// <summary>
/// 牌基类
/// </summary>
public class BaseCard 
{
    private string cardName;
    private int cardId;
    private CardColors cardColor;
    private CardWeight cardWeight;
    private CardBelongTo cardBelongTo;
    private CardDisplay cardDisplay;

    /// <summary>
    /// 卡牌名字
    /// </summary>
    public string CardName//GetCardName
    {
        get { return cardName; }
        private set { this.cardName = value; }
    }

    /// <summary>
    /// 卡牌ID
    /// </summary>
    public int CardId 
    {
        get { return cardId; }
        private set 
        {
            if (value>=0 && value<=25)
            {
                this.cardId = value;
            }
        }
    }

    /// <summary>
    /// 卡牌花色
    /// </summary>
    public CardColors CardColor
    {
        get { return cardColor; }
        private set { this.cardColor = value; }
    }

    /// <summary>
    /// 卡牌权值
    /// </summary>
    public CardWeight CardWeight
    {
        get { return cardWeight; }
        private set { this.cardWeight = value; }
    }

    /// <summary>
    /// 卡牌的归属
    /// </summary>
    public CardBelongTo CardBelongTo
    {
        get { return cardBelongTo; }
        set { cardBelongTo = value; }
    }

    /// <summary>
    /// 卡牌的显隐
    /// </summary>
    public CardDisplay CardDisplay
    {
        get { return cardDisplay; }
        set { cardDisplay = value; }
    }



    public BaseCard() { }
}