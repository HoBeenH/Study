namespace Script.Parameter.Enum
{
    public enum EMessageBoxBtnFlag
    {
        Ok,
        Cancel,
        Exit,
        BG,
        
        OK_Cancel = Ok | Cancel,
        OK_Cancel_BG = OK_Cancel | BG,
        All = OK_Cancel_BG | Exit
    }
}