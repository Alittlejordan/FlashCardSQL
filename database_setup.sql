-- Create Stacks table
CREATE TABLE Stacks (
    StackId INT PRIMARY KEY,
    StackName NVARCHAR(255) NOT NULL
);

-- Create FlashCards table with foreign key reference to Stacks
CREATE TABLE FlashCards (
    FlashcardId INT PRIMARY KEY,
    StackId INT FOREIGN KEY REFERENCES Stacks(StackId) ON DELETE CASCADE,
    Question NVARCHAR(MAX) NOT NULL,
    Answer NVARCHAR(MAX) NOT NULL
);