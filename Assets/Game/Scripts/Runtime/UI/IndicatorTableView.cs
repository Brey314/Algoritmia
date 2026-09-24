using System.Collections.Generic;
using Game.Reporting;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// La tabla de los cuatro indicadores, por nivel y por fase (RF-46, INC-35): el único lugar
    /// del juego donde una cifra es correcta (CP-03). Una fila por fase, clonada de una plantilla
    /// —el mismo patrón que <see cref="LevelSummaryController"/> usa para sus viñetas—, así que un
    /// nivel con más fases que otro simplemente añade más filas.
    /// </summary>
    public class IndicatorTableView : MonoBehaviour
    {
        [SerializeField] private ReportContent content;
        [SerializeField] private Transform rowsContainer;

        [Tooltip("Fila plantilla, inactiva; se clona por fase. Sus hijos se buscan por nombre: " +
                 "Nivel, Fase, Intentos, ErroresCorregidos, PasosUtilizados, Tiempo.")]
        [SerializeField] private GameObject rowTemplate;

        private readonly List<GameObject> _rows = new List<GameObject>();

#if UNITY_INCLUDE_TESTS
        internal IReadOnlyList<GameObject> Rows => _rows;
#endif

        internal void Render(IReadOnlyList<LevelReportSection> sections)
        {
            Clear();
            foreach (var section in sections)
            {
                foreach (var phase in section.Phases)
                {
                    var row = Instantiate(rowTemplate, rowsContainer);
                    row.name = $"Fila({section.Level}/{phase.Phase.Phase})";
                    row.SetActive(true);

                    SetText(row, "Nivel", content.LevelLabel(section.Level));
                    SetText(row, "Fase", content.PhaseLabel(phase.Phase.Phase));
                    SetText(row, "Intentos", ValueOrNoData(phase, phase.Indicators.Attempts.ToString()));
                    SetText(row, "ErroresCorregidos", ValueOrNoData(phase, phase.Indicators.CorrectedErrors.ToString()));
                    SetText(row, "PasosUtilizados", ValueOrNoData(phase, phase.Indicators.StepsUsed.ToString()));
                    SetText(row, "Tiempo", ValueOrNoData(phase, ResolutionTimeFormat.MinutesAndSeconds(phase.Indicators.ResolutionSeconds)));

                    _rows.Add(row);
                }
            }
        }

        private string ValueOrNoData(PhaseIndicators phase, string value) =>
            phase.Played ? value : content.NoDataLabel;

        private void Clear()
        {
            foreach (var row in _rows)
            {
                Destroy(row);
            }

            _rows.Clear();
        }

        private static void SetText(GameObject row, string childName, string value)
        {
            var child = row.transform.Find(childName);
            var text = child != null ? child.GetComponent<Text>() : null;
            if (text != null)
            {
                text.text = value;
            }
        }
    }
}
