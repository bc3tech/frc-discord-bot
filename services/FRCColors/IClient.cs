namespace FRCColors;

using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

public interface IClient
{
    (Color? primaryColor, Color? secondaryColor) GetColorsForTeam(ushort? teamNumber, CancellationToken cancellationToken);
    Task<(Color? primaryColor, Color? secondaryColor)> GetColorsForTeamAsync(ushort? teamNumber, CancellationToken cancellationToken);
}