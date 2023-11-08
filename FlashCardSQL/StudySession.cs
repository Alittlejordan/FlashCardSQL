using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCardSQL
{
    internal class StudySession
    {
        //this varirable is used to store the study session id
        public int StudySessionId { get; set; }

        //this variable is used to store the stack id
        public int StackId { get; set; }

        //this variable is used to store the date
        public DateTime Date { get; set; }

        //this variable is used to store the score
        public int Score { get; set; }
    }
}
