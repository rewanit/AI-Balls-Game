using System.Text;

namespace ConsoleApp3
{
    public class State
    {
        
        public enum MoveAction
        {
            Row,
            Column
        }
       
        public State(Field parentField, State parentState = null, MoveAction? action = null, int? actionId = null)
        {
            Field = parentField;
            ParentState = parentState;
            Action = action;
            ActionId = actionId;
        }

        public State? ParentState { get; set; }
        public MoveAction? Action { get; set; }
        public int? ActionId { get; set; }
        public Field Field { get; set; }

        public List<State> ChildStates { get; set; } = new List<State>();

        private void GenStates()
        {
            for (int i = 0; i < Field.Array.Length; i++)
            {
                var newField = Field.MoveCol(i);
                //if (!FieldsPool.Any(x=>x.Equals(newField)))
                {
                  //  FieldsPool.Add(newField);
                    var newState = new State(newField, this, MoveAction.Column, i);
                    ChildStates.Add(newState);
                }
            }
            for (int i = 0; i < Field.Array.Length; i++)
            {
                var newField = Field.MoveRow(i);
                //if (!FieldsPool.Any(x=>x.Equals(newField)))
                {
                  //  FieldsPool.Add(newField);
                    var newState = new State(newField, this, MoveAction.Row, i);
                    ChildStates.Add(newState);
                }
            }

        }

        static  public void Find(Field TargetField,List<State> pool)
        {
            var initState = pool.First();
            //todo save result
            while(!pool.Any(x => x.Field.Equals(TargetField)))
            {
                foreach (var item in pool)
                {
                    item.GenStates();
                }

                pool = pool.SelectMany(x => x.ChildStates).ToList();
                GC.Collect();
            }

            var result = pool.First(x => x.Field.Equals(TargetField));

            Console.WriteLine("Последовательное решение");

            var listToPrint = new List<string>();

            while (result.ParentState!=null)
            {
                listToPrint.Add(result.Print());
                result = result.ParentState;
            }

            listToPrint.Reverse();
            listToPrint.Insert(0, initState.Print());

            foreach (var item in listToPrint)
            {
                Console.WriteLine(item);
            }

        }

        public string Print()
        {
            var array = this.Field.Array;
            int rows = array.Length;
            int columns = array[0].Length;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (Action.HasValue)
                        if (Action.Value == MoveAction.Row && i == ActionId || Action.Value == MoveAction.Column && j == ActionId)
                        {
                            sb.Append("[");
                        }

                    sb.Append(array[i][j]);

                    if (Action.HasValue)
                        if (Action.Value == MoveAction.Row && i == ActionId || Action.Value == MoveAction.Column && j == ActionId)
                        {
                            sb.Append("]");
                        }
                   

                    if (j < columns - 1)
                    {
                        sb.Append("\t"); // Разделитель между элементами в строке
                    }
                }
               
                sb.AppendLine(); // Переход на следующую строку
            }
            sb.AppendLine("------------------------------------------------");
            return sb.ToString();
        }
    }
}