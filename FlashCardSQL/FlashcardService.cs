using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCardSQL
{
    internal class FlashcardService
    {
        private static string flashcardsConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        //this method is used to create a flashcard and add it to the database
        public void CreateFlashcard(int stackId, string question, string answer)
        {
            using (SqlConnection connection = new SqlConnection(flashcardsConnectionString))
            {
                connection.Open();
                string insertFlashcardQuery = "INSERT INTO Flashcards (StackId, Question, Answer) VALUES (@StackId, @Question, @Answer);";

                using (SqlCommand command = new SqlCommand(insertFlashcardQuery, connection))
                {
                    command.Parameters.AddWithValue("@StackId", stackId);
                    command.Parameters.AddWithValue("@Question", question) ;
                    command.Parameters.AddWithValue("@Answer", answer);

                    command.ExecuteNonQuery();
                }
            }
        
    }

        //this method is used to get the flashcards from the database
        public List<Flashcard> GetFlashcards(int stackId)
        {
            List<Flashcard> flashcards = new List<Flashcard>();

            using (var connection = new SqlConnection(flashcardsConnectionString))
            {
                connection.Open();

                string query = "SELECT FlashcardId, StackId, Question, Answer FROM Flashcards WHERE StackId = @StackId ORDER BY FlashcardId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StackId", stackId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Flashcard flashcard = new Flashcard
                            {
                                FlashcardId = Convert.ToInt32(reader["FlashcardId"]),
                                StackId = Convert.ToInt32(reader["StackId"]),
                                Question = reader["Question"].ToString(),
                                Answer = reader["Answer"].ToString()
                            };

                            flashcards.Add(flashcard);
                        }
                    }
                }
            }

            return flashcards;
        }
    }



    
}
