------------------------------------------- 
-- Delete all data from tables
------------------------------------------- 
--DELETE FROM packages;
--DELETE FROM cards;
--DELETE FROM users;

--SELECT id, name, owner FROM cards WHERE owner IS NOT NULL ORDER BY owner;

------------------------------------------- 
-- Select 4 cards for each players deck
------------------------------------------- 
--UPDATE cards SET deck = false;
--UPDATE cards SET deck = true WHERE id IN (SELECT id FROM cards WHERE owner = 'kienboec' LIMIT 4);
--UPDATE cards SET deck = true WHERE id IN (SELECT id FROM cards WHERE owner = 'altenhof' LIMIT 4);
--SELECT * from cards WHERE deck=true ORDER BY owner;
