<Query Kind="Program">
  <NuGetReference>Jint</NuGetReference>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>Jint</Namespace>
</Query>

void Main()
{

	string alCode = @"
  local procedure WaitForStartedUpdateFeatureUptakeSession()
    var
        StartDateTime: DateTime;
        Timeout: Duration;
    begin
        StartDateTime := CurrentDateTime();
        while IsSessionActive(StartedSessionId) do begin
            if CurrentDateTime - StartDateTime > Timeout then begin
                StartedSessionId := 0;
                Session.LogMessage('0000LKY', StartedSessionHasNotEndedErr, Verbosity::Warning, DataClassification::SystemMetadata, TelemetryScope::ExtensionPublisher, 'Category', TelemetryLibraryCategoryTxt);
                exit;
            end;

            Sleep(10); 
        end;
        StartedSessionId := 0;
    end;";
	object formattedCode = ALFormatter.GetTextAlFormat(alCode, true);
	formattedCode.Dump();
	Util.ClearResults();
	formattedCode = ALFormatter.GetTextAlFormatHtml(alCode, false);
	formattedCode.Dump();
}

public static string JSEngineRoot = @"c:\JSEngine\highlightjs\";
public static class ALFormatter
{
	private static Engine jsEngine; // Jint, Version=4.0.2.0
	private static string hlJsCode, hlJsCss;  // https://highlightjs.org/demo	
	private static object _lock = new object();
	public static string GetTextAlFormat(string alCode, bool forceReload = false)
	{
		lock (_lock) if (hlJsCode == null || forceReload)
			{
				hlJsCode = File.ReadAllText($@"{JSEngineRoot}\highlight.js");

				hlJsCss = File.ReadAllText($@"{JSEngineRoot}\styles\default.css")
						  //style is important.
						  + File.ReadAllText($@"{JSEngineRoot}\styles\vs.css");
				jsEngine = new Engine().SetValue("console",
					new
					{
						log = new Action<object>(Console.WriteLine),
						error = new Action<object>(Console.Error.WriteLine)
					})
				//for bc, delphi.js / cal.js exist out of the box.				
				.Execute(hlJsCode).Execute(File.ReadAllText($@"{JSEngineRoot}\languages\al.js"));
			}
		var html = jsEngine.Invoke("highlight", alCode, new { language = "al" }).Get("value").ToString();
		return html;
	}
	public static object GetTextAlFormatHtml(string alCode, bool forceReload = false)
	{
		var obj = Util.RawHtml($@"
			<style>{hlJsCss}</style>
			<pre>
				<span class=""hljs language-al"">
					<code>
						{GetTextAlFormat(alCode, forceReload)}
					</code>
				</span>
			</pre>");
		return obj;
	}
	/*
If you get regexp issue, 
changing this line in highlight.js, in function python(hljs) { (~line 45066) for Highlight.js v11.10.0
  const IDENT_RE = /[\p{XID_Start}_]\p{XID_Continue}.*snip*
to 
  const IDENT_RE = '';
	*/
}
