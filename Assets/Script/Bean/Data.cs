using System.Collections.Generic;
/// <summary>
/// 数据信息
/// </summary>
public class Data
{
    //开放信息
    public static UserInfo UsersInfo;//用户信息
    //持久化信息
    public UserInfo _UsersInfo;//用户信息
    //文件名称
    const string _FileName = "data.sav";

    //实例化新数据
    public static Data Instance()
    {
        Data data = new Data();
        UserInfo userinfo = new UserInfo();
        userinfo.Sound = true;
        userinfo.BGM = true;

        data._UsersInfo = userinfo;
        Logger.WARM("Instance Data");
        return data;
    }

    //存储数据
    public static void SaveData()
    {
        Data data = new Data();
        data._UsersInfo = UsersInfo;
        Persistence.SaveData(data);
    }

    //加载数据
    public static void LoadData()
    {
        Data data = Persistence.LoadData();
        if (data == null) data = Instance();
        UsersInfo = data._UsersInfo;
    }

    #region 数据类型
    //用户数据
    public class UserInfo
    {
        #region 游戏设置
        public bool Sound;//音效开关
        public bool BGM;//音乐开关

        public int Gold = 0;//金币
        #endregion
    }
    #endregion
}
