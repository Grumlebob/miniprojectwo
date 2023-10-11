namespace miniprojectwo;

public class Person
{
    private int privateValue { get; set; }

    public Person(int privateValue)
    {
        this.privateValue = privateValue;
    }

    public int[] calculateShares()
    {
        Random random = new Random();
        int target = privateValue;
        int remaining = target;
        int[] randomNumbers = new int[3];

        for (int i = 0; i < randomNumbers.Length - 1; i++)
        {
            int randomNumber = random.Next(-remaining, remaining + 1);
            randomNumbers[i] = randomNumber;
            remaining -= randomNumber;
        }

        randomNumbers[randomNumbers.Length - 1] = remaining;

        // Shuffle the array to make the numbers random
        for (int i = 0; i < randomNumbers.Length; i++)
        {
            int temp = randomNumbers[i];
            int randomIndex = random.Next(i, randomNumbers.Length);
            randomNumbers[i] = randomNumbers[randomIndex];
            randomNumbers[randomIndex] = temp;
        }

        return randomNumbers;
    }
    
}