codeunit 69102 "PTE Addr. 3 Testfields"
{
    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Release Sales Document", 'OnBeforeReleaseSalesDoc', '', false, false)]
    [ErrorBehavior(ErrorBehavior::Collect)]
    local procedure OnBeforeReleaseSalesDoc(var SalesHeader: Record "Sales Header"; PreviewMode: Boolean; var IsHandled: Boolean)
    var
        Customer: Record Customer;
        SalesLine: Record "Sales Line";
        ErrMsg: Text;
        Err: ErrorInfo;
        TypeHelper: Codeunit "Type Helper";
    begin
        if PreviewMode then
            exit;

        if not Customer.Get(SalesHeader."Sell-to Customer No.") then
            exit;

        Customer.TestField("Address 2", ErrorInfo.Create());

        SalesHeader.TestField("Ship-to Address 2", ErrorInfo.Create());

        SalesHeader.TestField("Bill-to Address 2", ErrorInfo.Create('Parameter has to be set to true (collectible), else the error is thrown immediately.', true));

        SalesHeader.TestField("Sell-to Address 2", ErrorInfo.Create());

        ErrMsg := '';
        foreach Err in GetCollectedErrors(true) do
            ErrMsg += Err.Message + TypeHelper.NewLine() + TypeHelper.NewLine();
        if ErrMsg <> '' then Error(ErrMsg);
    end;
}