using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using System.Collections;

namespace SlideSeparator
{
	public class SlideTextPortions : MonoBehaviour
    {
        private static short lineMaxLenght = 45;  	//the max length of a line
        private static short linesInSlide = 10;	//number of lines in each slide
        //private static short slidesMaxNo = 40; 	//the max number of slides for one table in the room

		[Tooltip("Array with texts of all the slides.")]
		//public string[] textSlidePortions;
		public string[] textSlidePortions;
		[Tooltip("Number of all the text slides.")]
		public short slidePortionsNo;
		[Tooltip("Number of current slide shown at the board.")]
        public short currentSlideShown;

		private ArrayList textSlidePortionsList;
		private string remainder;
		private string quotient;

		public SlideTextPortions()
		{
			textSlidePortionsList = new ArrayList();
		}

		public void SetInputText(string rawText)
		{
			SetSlidePortions(rawText);
		}

		public string SetNext()
		{
			if (currentSlideShown < (slidePortionsNo-1)) {
				currentSlideShown++;
			}
			return textSlidePortions[currentSlideShown];
		}

		public string SetPrev()
		{
			if (currentSlideShown > 0) {
				currentSlideShown--;
			}
			return textSlidePortions[currentSlideShown];
		}

		public string SetFirst()
		{
			currentSlideShown = 0;

			return textSlidePortions[currentSlideShown];
		}

		public string SetLast()
		{
			currentSlideShown = slidePortionsNo; 
			currentSlideShown--;
			return (string) textSlidePortions[currentSlideShown];
		}

        private void SetSlidePortions(string rawText)
        {
            //textSlidePortions = new string[slidesMaxNo];
            string[] textLines = GetTextLinesWithRequiredLength(rawText);

            short controlLineNumbers = 1;
            short textSlidePortionsNumber = 0;
            string bufferText = string.Empty;

            for (int i = 0; i < textLines.Length; i++)
            {
				bufferText += textLines[i] + "\n";
				controlLineNumbers++;

				if ((controlLineNumbers == linesInSlide+1 /*we have a full slide*/) | 
					(i == textLines.Length - 1 /*we reached last line for this board*/))
                {
                    //textSlidePortions[textSlidePortionsNumber] = bufferText;
					textSlidePortionsList.Add(bufferText);
                    textSlidePortionsNumber++;

                    controlLineNumbers = 1;
                    bufferText = string.Empty;
                }
				/*
				DivideText(textLines[i], lineMaxLenght);
				bufferText += quotient;

                if (i == textLines.Length - 1)
                {
                    textSlidePortions[textSlidePortionsNumber] = bufferText;
                }
                else
                {
                    if (remainder != null && remainder.Length > 0)
                    {
                        bufferText += remainder;
                    }

                    bufferText += Environment.NewLine;
                }*/
				
            }

			textSlidePortions = textSlidePortionsList.ToArray (typeof(string)) as string[];
			slidePortionsNo = textSlidePortionsNumber;
			currentSlideShown = 0;
        }

		private void DivideText(string givenString, int numberSymbols)
        { 
            if (givenString.Length < numberSymbols)
            {
                numberSymbols = givenString.Length;
                quotient = givenString;
                remainder = string.Empty;
            }
            else
            {
                string targetString = givenString.Substring(0, numberSymbols);
				bool endCorrect = targetString.EndsWith("\n") || targetString.EndsWith(".") || targetString.EndsWith(",") || targetString.EndsWith("-") || targetString.EndsWith(":") || targetString.EndsWith(";") || Char.IsWhiteSpace(targetString[numberSymbols - 1]);
                if (endCorrect)
                {
                    quotient = targetString;
                    remainder = givenString.Substring(numberSymbols).TrimStart();
                }
                else
                {
                    int lastWhitespaceIndex = targetString.LastIndexOf(" ");
                    quotient = givenString.Substring(0, lastWhitespaceIndex);
                    remainder = givenString.Substring(lastWhitespaceIndex).TrimStart();
                } 
            }
        }

		private string[] GetTextLinesWithRequiredLength(string rawText)
        {
			//List<string> rawTextLines = new List<string>(rawText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
			List<string> rawTextLines = new List<string>(Regex.Split(rawText, "\n"));
            List<string> result = new List<string>();
            foreach (string line in rawTextLines)
            {
				DivideText(line, lineMaxLenght);
                result.Add(quotient);
                if (remainder != null && remainder.Length > 0)
                {
                    result.AddRange(GetTextLinesWithRequiredLength(remainder));
                }
            }

            return result.ToArray();
        }
    }
}