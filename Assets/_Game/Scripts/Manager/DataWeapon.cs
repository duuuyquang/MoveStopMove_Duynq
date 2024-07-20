using System.Collections.Generic;

public static class DataWeapon
{
    private static List<Dictionary<string, string>> datas = new List<Dictionary<string, string>>();

    public static void Init()
    {
        //datas = CSVReader.GetData("Weapons");
    }

    public static List<Dictionary<string, string>> FetchAll()
    {
        return datas; 
    }
    
    public static DataWeaponEach FetchByID(int id)
    {
        return new DataWeaponEach(datas[id]);
    }
}

public class DataWeaponEach
{
    private int     id;
    private int     atkRange;
    private int     atkSpeed;
    private bool    isReturn;
    private int     coin;
    private bool    isPurchased;

    public int ID { get { return id; } }
    public int AtkRange { get { return atkRange; } }
    public int AtkSpeed {  get { return atkSpeed; } }
    public bool IsReturn {  get { return isReturn; } }
    public int Coin { get { return coin; } }
    public bool IsPurchased {  get { return isPurchased; }  set { isPurchased = value; } }

    public DataWeaponEach(Dictionary<string, string> data)
    {
        id = int.Parse(data["Id"]);
        atkRange = int.Parse(data["AtkRange"]);
        atkSpeed = int.Parse(data["AtkSpeed"]);
        isReturn = int.Parse(data["Return"]) == 0 ? false : true;
        coin = int.Parse(data["Coin"]);
        isPurchased = int.Parse(data["IsPurchased"]) == 0 ? false : true;
    }

}
