using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    public Reel[] reels;
    public int credits = 0;

    private readonly int[][] lines = new int[][]
    {
        new int[]{0,1,2},
        new int[]{3,4,5},
        new int[]{6,7,8},
        new int[]{0,4,8},
        new int[]{2,4,6}
    };

    public void Spin()
    {
        foreach (var reel in reels)
        {
            reel.Spin();
        }
        Evaluate();
    }

    private void Evaluate()
    {
        SlotSymbol[] grid = new SlotSymbol[9];
        for (int r = 0; r < reels.Length; r++)
        {
            var reelSymbols = reels[r].currentSymbols;
            if (reelSymbols == null || reelSymbols.Length < 3)
                continue;
            for (int c = 0; c < 3; c++)
            {
                grid[r*3 + c] = reelSymbols[c];
            }
        }

        foreach (var line in lines)
        {
            SlotSymbol first = grid[line[0]];
            if (first == null) continue;
            bool allSame = true;
            for (int i = 1; i < line.Length; i++)
            {
                if (grid[line[i]] != first)
                {
                    allSame = false;
                    break;
                }
            }
            if (allSame)
            {
                credits += first.payout;
            }
        }

        int bonusCount = 0;
        foreach (var symbol in grid)
        {
            if (symbol != null && symbol.isBonus)
            {
                bonusCount++;
            }
        }
        if (bonusCount >= 3)
        {
            credits += 10 * bonusCount;
        }
    }
}
