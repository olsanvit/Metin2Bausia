using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Metin2Bausia.Web.Components;

/// <summary>
/// Základ všech stránek adminu (přes Pages/_Imports.razor).
/// Proč: stránky dřív výjimky jen zobrazily a zahodily — v logu kontejneru pak po chybě nezůstala stopa.
/// Toast navíc drží počítadlo, aby se stejná hláška (dvakrát „Uloženo") zobrazila znovu a
/// běžné překreslení (psaní do vyhledávání) ji naopak neoživilo.
/// </summary>
public abstract class AdminPageBase : ComponentBase
{
    [Inject] private ILoggerFactory LoggerFactory { get; set; } = default!;
    [Inject] private IStringLocalizer<SharedResources> Loc { get; set; } = default!;

    protected string? ToastText { get; private set; }
    protected bool ToastIsError { get; private set; }
    protected int ToastStamp { get; private set; }

    protected void ShowOk(string text)
    {
        ToastText = text;
        ToastIsError = false;
        ToastStamp++;
    }

    protected void ShowError(Exception ex, [CallerMemberName] string operation = "")
    {
        LogError(ex, operation);
        ToastText = Loc["Error", ex.Message];
        ToastIsError = true;
        ToastStamp++;
    }

    /// <summary>Zaloguje výjimku a vrátí text pro zobrazení na stránce.</summary>
    protected string LogError(Exception ex, [CallerMemberName] string operation = "")
    {
        LoggerFactory.CreateLogger(GetType()).LogError(ex, "{Page}.{Operation} selhalo", GetType().Name, operation);
        return ex.Message;
    }
}
