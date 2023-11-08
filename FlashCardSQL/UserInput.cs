using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FlashCardSQL
{
    internal class UserInput
    {

        //this method will create the menu for the user to select from
        public static void CreateMenu()
        {


            Console.WriteLine("\nWelcome to FlashCardSQL");
            Console.WriteLine("Please select an option:");
            Console.WriteLine("1. Create a new stack");
            Console.WriteLine("2. View stacks");
            Console.WriteLine("3. Delete a stack");
            Console.WriteLine("4. Study a stack");
            Console.WriteLine("5. View Study Session");
            Console.WriteLine("6. Exit");
            Console.WriteLine("Enter your selection: \n");
            string userInput = Console.ReadLine();
            int userSelection = Convert.ToInt32(userInput);
            //this switch statement will take the user input and call the appropriate method
            switch (userSelection)
            {
                case 1:
                    CreateStack();
                    break;
                case 2:
                    ViewStacks();
                    break;
                case 3:
                    DeleteStack();
                    break;
                case 4:
                    StudyStack();
                    break;

                case 5:
                    ViewStudySession();
                                           
                    break;
                case 6:
                    Exit();
                    break;
                default:
                    Console.WriteLine("Invalid selection. Please try again.");
                    CreateMenu();
                    break;
            }
        }

        //this method will delete a stack
        private static void DeleteStack()
        {
            StackService stackRepository = new StackService();
            // Prompt the user for the stack to delete
            int stackIdToDelete = GetIntInput("Enter the Stack ID to delete: ");

            //this checks to see if the stack exists
            Stack stack= stackRepository.GetStackByID(stackIdToDelete);

            //if the stack does not exist, the user will be prompted to enter a valid stack ID
            if(stack==null)
            {
                Console.WriteLine($"Error: Stack with ID {stackIdToDelete} not found.");
                CreateMenu();
            }
            stackRepository.DeleteStack(stackIdToDelete);
            Console.WriteLine($"Stack with ID {stackIdToDelete} and associated flashcards deleted successfully.");
            CreateMenu();

          
        }

        //this method will view the study sessions
        private static void ViewStudySession()
        {
            StudySessionService studySessionRepository = new StudySessionService();

            //this will get all the study sessions
            List<StudySession> allStudySessions = studySessionRepository.GetStudySessions();

            //this will check to see if there are any study sessions
            if(allStudySessions.IsNullOrEmpty())
            {
                 Console.WriteLine("No study sessions found.\n");
                CreateMenu();
            }

            // Display study sessions to the user
            Console.WriteLine("\nAll Study Sessions:");
            foreach (var studySession in allStudySessions)
            {
                Console.WriteLine($"StudySessionID: {studySession.StudySessionId}, StackID: {studySession.StackId}, Date: {studySession.Date}, Score: {studySession.Score}");
            }
            CreateMenu();
        }

        //this method will start a study session
        private static void StudyStack()
        {
            StudySessionService studySession = new StudySessionService();
            Console.WriteLine("\nPlease enter the name of the stack you would like to study: ");

            string stackName = Console.ReadLine();
            StackService Repository = new StackService();
            if (int.TryParse(stackName, out int selectedStackId))
            {
                // Retrieve the selected stack and its flashcards
                Stack selectedStack = Repository.GetStackByID(selectedStackId);

                if (selectedStack == null)
                {
                    Console.WriteLine("Please enter a valid stack ID.");
                    StudyStack();
                 }

                StartStudySession(selectedStackId);

            }

           
        }
    
        //this method will exit the program
        private static void Exit()
        {
            Console.WriteLine("\nThank you for using FlashCardSQL. Goodbye!");
            Environment.Exit(0);
            
        }

        //this method will view the stacks
        private static void ViewStacks()
        {
            StackService Repository = new StackService();

            // Display available stacks
            List<Stack> stacks = Repository.GetStacks();
            Console.WriteLine("Available Stacks:");
            foreach (var stack in stacks)
            {
                Console.WriteLine($"StackID: {stack.StackId}, StackName: {stack.StackName}");
            }

            // Ask the user to select a stack
            Console.Write("\nEnter the StackID you want to view:\n ");
            if (int.TryParse(Console.ReadLine(), out int selectedStackId))
            {
                // Retrieve the selected stack and its flashcards
                Stack selectedStack = Repository.GetStackByID(selectedStackId);

                if (selectedStack != null)
                {
                    // Display the selected stack and its flashcards
                    Console.WriteLine($"Selected Stack:\n {selectedStack.StackName}");
                    foreach (var flashcard in selectedStack.Flashcards)
                    {
                        Console.WriteLine($"\n\n  FlashcardID: {flashcard.FlashcardId}, Question: {flashcard.Question}, Answer: {flashcard.Answer}\n");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid StackID. Stack not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid StackID.");
                ViewStacks();
            }
            CreateMenu();
        }
    


        private static void CreateStack()
        {
            Console.WriteLine("Please enter the name of the tack you would like to create: ");

            string stackName = Console.ReadLine();

            if (stackName == "")
            {
                Console.WriteLine("Please enter a valid stack name.");
                CreateStack();
            }
            else
            {

                CreateFlashCardsWithStack(stackName);


            }
        }

        //this method will create a stack and add flashcards to it
        private static void CreateFlashCardsWithStack(string stackName)
        {
            Stack stack = new Stack
            {
                StackName = stackName,
                Flashcards = new List<Flashcard>(),
            };

            bool doneAddingFlashcards = false;
            StackService service = new StackService();
            do
            {
                Console.WriteLine("Enter flashcard details:");
                Console.Write("Question: ");
                string question = Console.ReadLine();

                while (question == "")
                {
                    Console.WriteLine("Please enter a valid answer.");
                    question = Console.ReadLine();
                }

                Console.Write("Answer: ");
                string answer = Console.ReadLine();
                while (answer == "")
                {
                    Console.WriteLine("Please enter a valid answer.");
                    answer = Console.ReadLine();
                }

                service.AddFlashcardToStack(stack, question, answer);

                Console.Write("Do you want to add another flashcard? (y/n): ");
                string userInput = Console.ReadLine();

         

                doneAddingFlashcards = !userInput.Trim().Equals("y", StringComparison.OrdinalIgnoreCase);

            } while (!doneAddingFlashcards);

            // Now, you can save the entire stack with flashcards to the database
            service.SaveStackAndFlashcardsToDatabase(stack);
            CreateMenu();
        }

        public static void StartStudySession(int stackId)
        {
            FlashcardService flashcardRepository = new FlashcardService();
            List<Flashcard> flashcards = flashcardRepository.GetFlashcards(stackId);

            if (flashcards.Count == 0)
            {
                Console.WriteLine("No flashcards found for the selected stack.");
                return;
            }

            Console.WriteLine($"Starting study session for Stack ID: {stackId}");
            int score = 0;
            foreach (var flashcard in flashcards)
            {
                Console.WriteLine($"Flashcard ID: {flashcard.FlashcardId}");
                Console.WriteLine($"Question: {flashcard.Question}");

                // Capture user's response
                Console.Write("Your answer: ");
                string userAnswer = Console.ReadLine();

                // Compare user's answer with the correct answer
                bool isCorrect = userAnswer.Trim().Equals(flashcard.Answer.Trim(), StringComparison.OrdinalIgnoreCase);

                if (isCorrect)
                {
                    Console.WriteLine("Correct!");
                    // Record the result,
                    score++;
                }
                else
                {
                    Console.WriteLine($"Incorrect. The correct answer is: {flashcard.Answer}");
                    score--;
                }

                Console.WriteLine();
            }

            Console.WriteLine("Study session completed.");
            Console.WriteLine($"Score: {score}");

            StudySessionService studySessionRepository = new StudySessionService();

            studySessionRepository.RecordStudySession(stackId, score);
            CreateMenu();
            
        }

        //this method will check to see if the user input is an integer
        public static int GetIntInput(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);

                if (int.TryParse(Console.ReadLine(), out int result))
                {
                    return result;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid integer.");
                }
            }
        }
    }

}
