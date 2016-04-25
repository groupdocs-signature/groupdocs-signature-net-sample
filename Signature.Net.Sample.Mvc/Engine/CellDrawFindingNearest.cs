using Aspose.Cells.Rendering;

namespace Signature.Net.Sample.Mvc.Engine
{
    internal class CellDrawFindingNearest : DrawObjectEventHandler
    {
        private int _left, _top;
        private int _smallestDistance;
        private bool _isFirstPass = true;
        public int Column { get; set; }
        public int Row { get; set; }

        public void SetPositions(int left, int top)
        {
            _left = left;
            _top = top;
        }

        public override void Draw(DrawObject drawObject, float x, float y, float width, float height)
        {
            var cell = drawObject.Cell;
            if (cell != null)
            {
                int horizontalDistance = (int)x - _left;
                int varticalDistance = (int)y - _top;
                int distance = horizontalDistance * horizontalDistance + varticalDistance * varticalDistance;
                if (_isFirstPass || distance < _smallestDistance)
                {
                    _smallestDistance = distance;
                    Column = cell.Column;
                    Row = cell.Row;
                    _isFirstPass = false;
                }
            }
        }
    }
}