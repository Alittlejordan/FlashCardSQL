-- Insert initial data into Stacks table
INSERT INTO Stacks (StackId, StackName) VALUES
(1, 'Mathematics'),
(2, 'History'),
(3, 'Science');

-- Insert initial data into FlashCards table
INSERT INTO FlashCards (FlashcardId, StackId, Question, Answer) VALUES
(1, 1, 'What is 2 + 2?', '4'),
(2, 1, 'Solve for x: 3x - 5 = 10', '5'),
(3, 2, 'Who was the first President of the United States?', 'George Washington'),
(4, 3, 'What is the chemical symbol for water?', 'H2O');