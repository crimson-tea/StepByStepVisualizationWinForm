using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StepByStepVisualizationWinForm
{
    internal interface IRedoUndo<TOperation>
    {
        internal void ExecuteRedo(TOperation operation);
        internal void ExecuteUndo(TOperation operation);
        internal void SetProgress(int step);
    }
}
