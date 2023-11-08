using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCardSQL
{
    internal class Flashcard
    {
        //this variable is used to store the flashcard id
        public int FlashcardId { get; set; }

        //this variable is used to store the stack id
        public int StackId { get; set; }

        //this variable is used to store the question
        public string Question { get; set; }

        //this variable is used to store the answer
        public string Answer { get; set; }
    }
}
