using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCardSQL
{
    internal class Stack
    {
        //this variable is used to store the stack id
        public int StackId { get; set; }

        //this variable is used to store the stack name
        public string StackName { get; set; }
        
        //this variable is used to store the flashcards
        public List<Flashcard> Flashcards { get; set; }
    }
}
