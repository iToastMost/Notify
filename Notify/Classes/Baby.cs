namespace Notify.Classes;

public class Baby
{
    public int BabyId { get; set; }
    public string BabyName { get; set; }
    public string BabyPassword { get; set; }

    public Baby(string babyName, string babyPassword)
    {
        BabyName = babyName;
        BabyPassword = babyPassword;
    }
}