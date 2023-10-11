using miniprojectwo;
using static miniprojectwo.Peers;


// Create instances for each peer
Alice = new Peer("Alice", 12345, 10);
Bob = new Peer("Bob", 12346, 8);
Charlie = new Peer("Charlie", 12347, 15);
Hospital = new Peer("Hospital", 12348,0);

// Start each peer
Thread aliceThread = new Thread(() => Alice.Start());
Thread bobThread = new Thread(() => Bob.Start());
Thread charlieThread = new Thread(() => Charlie.Start());
Thread hospitalThread = new Thread(() => Hospital.Start());

aliceThread.Start();
bobThread.Start();
charlieThread.Start();
hospitalThread.Start();

// Wait for all threads to finish
aliceThread.Join();
bobThread.Join();
charlieThread.Join();
hospitalThread.Join();

Alice.PrintAllFields();
Bob.PrintAllFields();
Charlie.PrintAllFields();
Hospital.PrintAllFields();

var aliceNoTLS = new Person(7);
var bobNoTLS = new Person(10);
var charlieNoTLS = new Person(17);

var AliceShares = aliceNoTLS.calculateShares();
var BobShares = bobNoTLS.calculateShares();
var CharlieShares = charlieNoTLS.calculateShares();

var aShareTotal = AliceShares.Sum();
foreach (var aShare in AliceShares)
{
    Console.WriteLine(aShare);
}
Console.WriteLine("Alice total 7. Shares added equals:" + aShareTotal);

var bShareTotal = BobShares.Sum();
foreach (var bShare in BobShares)
{
    Console.WriteLine(bShare);
}
Console.WriteLine("Bob total 10. Shares added equals:" + bShareTotal);

var cShareTotal = CharlieShares.Sum();
foreach (var cShare in CharlieShares)
{
    Console.WriteLine(cShare);
}
Console.WriteLine("Charlie total 17. Shares added equals:" + cShareTotal);


