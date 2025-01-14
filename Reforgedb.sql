-- Step 1: Check the structure of UserAccounts
PRAGMA table_info(UserAccounts);

-- Step 2: Create a new table with the same schema but with IsAdmin as BOOLEAN
CREATE TABLE UserAccounts_temp (
    Id INTEGER PRIMARY KEY,
    UserName TEXT NOT NULL,
    Password TEXT NOT NULL,
    Email TEXT NOT NULL,
    IsAdmin BOOLEAN NOT NULL DEFAULT 0,
    FirstName TEXT NOT NULL,
    LastName TEXT NOT NULL,
    RecuringDays TEXT NOT NULL
);

-- Step 3: Migrate data to the new table, converting IsAdmin values
INSERT INTO UserAccounts_temp (Id, UserName, Password, Email, IsAdmin, FirstName, LastName, RecuringDays)
SELECT Id, UserName, Password, Email, 
       CASE 
           WHEN IsAdmin = 'true' OR IsAdmin = 1 THEN 1 
           ELSE 0 
       END,
       FirstName, LastName, RecuringDays
FROM UserAccounts;

-- Step 4: Drop the old table
DROP TABLE UserAccounts;

-- Step 5: Rename the new table to UserAccounts
ALTER TABLE UserAccounts_temp RENAME TO UserAccounts;
