using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => Results.Content(@"<!DOCTYPE html>
<html>
<head>
    <title>Lottery Number Generator</title>
    <style>
        body { font-family: Arial; padding: 50px; background: linear-gradient(135deg, #e9dbf5 0%, #a682ca 100%); }
        .container { background: white; padding: 40px; border-radius: 15px; max-width: 500px; margin: 0 auto; box-shadow: 0 10px 30px rgba(0,0,0,0.3); text-align: center; }
        h1 { color: #333; margin-bottom: 30px; }
        button { background: #667eea; color: white; border: none; padding: 15px 40px; font-size: 18px; border-radius: 8px; cursor: pointer; }
        button:hover { background: #764ba2; }
        .numbers { margin-top: 30px; display: none; }
        .ball { display: inline-block; width: 60px; height: 60px; line-height: 60px; background: #667eea; color: white; border-radius: 50%; margin: 5px; font-size: 24px; font-weight: bold; }
        .bonus { background: #e74c3c; }
        .label { margin-top: 20px; color: #666; font-size: 14px; }
        .winning { margin-top: 40px; padding-top: 30px; border-top: 2px solid #eee; display: none; }
        .winning h2 { color: #667eea; font-size: 20px; }
        .winning-numbers { font-size: 24px; color: #333; font-weight: bold; margin-top: 10px; }

    </style>
</head>
<body>
    <div class='container'>
        <h1>Lottery Number Generator</h1>
        <button onclick='generateNumbers()'>Generate Numbers</button>
        <div class='numbers' id='numbers'></div>
    </div>


    <script>
        
        function animateNumber(element, finalNumber) {
            let current = 1;
            const interval = setInterval(() => {
                element.textContent = current;
                current++;
                if (current > 60) current = 1;
            }, 30);
            
            setTimeout(() => {
                clearInterval(interval);
                element.textContent = finalNumber;
            }, 1000);
        }
        
        function shuffleArray(arr) { for (var i = arr.length - 1; i > 0; i--) { var j = Math.floor(Math.random() * (i + 1)); [arr[i], arr[j]] = [arr[j], arr[i]]; } return arr; }
        
        function generateNumbers() {
            fetch('/generate')
                .then(response => response.json())
                .then(data => {
                    var numbersDiv = document.getElementById('numbers');
                    numbersDiv.innerHTML = '';
                    numbersDiv.style.display = 'block';
                    
                    var mainDiv = document.createElement('div');
                    numbersDiv.appendChild(mainDiv);
                    
                    var colors = shuffleArray(['#e74c3c','#3498db','#2ecc71','#f39c12','#9b59b6','#1abc9c']);
                    
                    data.mainNumbers.forEach((num, index) => {
                        setTimeout(() => {
                            var ball = document.createElement('div');
                            ball.className = 'ball';
                            ball.style.background = colors[index];
                            ball.textContent = '1';
                            mainDiv.appendChild(ball);
                            animateNumber(ball, num);
                        }, index * 1500);
                    });
                    
                    setTimeout(() => {
                        var label = document.createElement('div');
                        label.className = 'label';
                        label.textContent = 'Main Numbers';
                        numbersDiv.appendChild(label);
                        
                        var bonusDiv = document.createElement('div');
                        var bonusBall = document.createElement('div');
                        bonusBall.className = 'ball bonus';
                        bonusBall.textContent = '1';
                        bonusDiv.appendChild(bonusBall);
                        numbersDiv.appendChild(bonusDiv);
                        animateNumber(bonusBall, data.bonusNumber);
                        
                        var bonusLabel = document.createElement('div');
                        bonusLabel.className = 'label';
                        bonusLabel.textContent = 'Bonus Number';
                        numbersDiv.appendChild(bonusLabel);
                    }, 9000);
                    


                });
        }
    </script>
</body>
</html>", "text/html"));

app.MapGet("/generate", () =>
{
    var random = new Random();
    var numbers = new List<int>();
    
    while (numbers.Count < 6)
    {
        var num = random.Next(1, 61);
        if (!numbers.Contains(num))
        {
            numbers.Add(num);
        }
    }
    
    var bonus = random.Next(1, 61);
    while (numbers.Contains(bonus))
    {
        bonus = random.Next(1, 61);
    }
    
    return Results.Json(new { mainNumbers = numbers.ToArray(), bonusNumber = bonus });
});

app.Run();
