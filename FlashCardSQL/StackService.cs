using Microsoft.IdentityModel.Protocols;
using NLog.Internal;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace FlashCardSQL
{
    internal class StackService
    {
        string stacksConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        //this method will save the stack and flashcards to the database
        public void SaveStackAndFlashcardsToDatabase(Stack stack)
        {

            using (SqlConnection connection = new SqlConnection(stacksConnectionString))
            {
                connection.Open();

                // Insert the stack
                string insertStackQuery = "INSERT INTO Stacks (StackName) " +
                    "VALUES (@StackName); SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(insertStackQuery, connection))
                {
                    command.Parameters.AddWithValue("@StackName", stack.StackName);
                    int stackId = Convert.ToInt32(command.ExecuteScalar());
                    stack.StackId = stackId;
                }

                // Create a new FlashcardService object
                FlashcardService flashcardService = new FlashcardService();
                // Insert the flashcards

                // Loop through the flashcards in the stack
                foreach (var flashcard in stack.Flashcards)
                {
                    // Call the CreateFlashcard method of the FlashcardService object
                    flashcardService.CreateFlashcard(stack.StackId, flashcard.Question, flashcard.Answer);
                }
            }
        }

        //this method will return a list of stacks
        public List<Stack> GetStacks()
        {
            List<Stack> stacks = new List<Stack>();

            using (SqlConnection connection = new SqlConnection(stacksConnectionString))
            {
                connection.Open();

                string query = "SELECT StackId, StackName FROM Stacks";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Stack stack = new Stack
                            {
                                StackId = Convert.ToInt32(reader["StackId"]),
                                StackName = reader["StackName"].ToString()
                            };

                            stacks.Add(stack);
                        }
                    }
                }
            }

            return stacks;
        }

        //this method will return a stack by id
        public Stack GetStackByID(int stackId)
        {
            Stack stack = new Stack();
            FlashcardService flashcardService = new FlashcardService();
           
            //connect to the database
            using (var connection = new SqlConnection(stacksConnectionString))
            {
                connection.Open();

                string query = "SELECT StackName FROM Stacks WHERE StackId = @StackId";

                //this command will get the stack name
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StackId", stackId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            stack.StackId = stackId;
                            stack.StackName = reader["StackName"].ToString();
                        }
                        else
                        {
                            return null; // Stack not found
                        }
                    }
                }

                // Retrieve flashcards for the stack
                stack.Flashcards =GetFlashcardsForStack(stackId);
            }

            return stack;
        }

        //this method will Delete a stack
        public void DeleteStack(int stackId)
        {
            using (SqlConnection connection = new SqlConnection(stacksConnectionString))
            {
                connection.Open();

                // Delete the stack and associated flashcards
                string deleteStackQuery = "DELETE FROM Stacks WHERE StackId = @StackId";

                try
                {
                    using (SqlCommand command = new SqlCommand(deleteStackQuery, connection))
                    {
                        command.Parameters.AddWithValue("@StackId", stackId);

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

              
            }
        
    }

        //add a flashcard to a stack
        public void AddFlashcardToStack(Stack stack, string question, string answer)
        {
            int nextFlashcardId;
            if (stack.Flashcards != null)
            {
                nextFlashcardId = stack.Flashcards.Count + 1;
            }
            else
                nextFlashcardId = 1;

            Flashcard flashcard = new Flashcard
            {
                FlashcardId = nextFlashcardId,
                StackId = stack.StackId,
                Question = question,
                Answer = answer
            };

            stack.Flashcards.Add(flashcard);
        }

        //get flashcards for a stack
        private List<Flashcard> GetFlashcardsForStack(int stackId)
        {
            List<Flashcard> flashcards = new List<Flashcard>();

            using (SqlConnection connection = new SqlConnection(stacksConnectionString))
            {
                connection.Open();

                string query = "SELECT FlashcardId, Question, Answer FROM Flashcards WHERE StackId = @StackId ORDER BY FlashcardId";

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
                                StackId = stackId,
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
