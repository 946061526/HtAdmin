/// <summary>
/// 彩种类型
/// </summary>
public enum GameCategory
{
    数字彩 = 0,
    传统足球 = 1,
    北单竞彩 = 3
}

public enum NoticeType
{
    /// <summary>
    /// 出票结果通知
    /// </summary>
    Ticketed = 10,
    /// <summary>
    /// 奖期通知
    /// </summary>
    Issuse = 20,
    /// <summary>
    /// 开奖通知
    /// </summary>
    Open = 30,
    /// <summary>
    /// 派奖通知
    /// </summary>
    Prize = 40,
    /// <summary>
    /// 传统足球队伍通知
    /// </summary>
    CTZQMatchNotice = 50,
    /// <summary>
    /// 传统足球奖池通知
    /// </summary>
    CTZQMatchPoolNotice = 60,
}