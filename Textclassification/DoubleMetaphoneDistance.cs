/**
 * DoubleMetaphoneDistance.cs
 *
 * An implemenatation of Lawrence Phillips' Double Metaphone phonetic matching
 * algorithm, published in C/C++ Users Journal, June, 2000.
 *
 * This implementation was written by Adam J. Nelson (anelson@nullpointer.net).
 * It is based on the C++ template implementation, also by Adam Nelson.
 * For the latest version of this implementation, implementations
 * in other languages, and links to articles I've written on the use of my various
 * Double Metaphone implementations, see:
 * http;//www.nullpointer.net/anelson/
 *
 * Note that since this impl implements IComparable, it can be used to key associative containers,
 * thereby easily implementing phonetic matching within a simple container.  Examples of this
 * should have been included in the archive from which you obtained this file.
 *
 * Current Version: 1.0.0
 * Revision History:
 * 	1.0.0 - ajn - First release
 *
 * This implemenatation, and optimizations, Copyright (C) 2003, Adam J. Nelson
 * The Double Metaphone algorithm was written by Lawrence Phillips, and is
 * Copyright (c) 1998, 1999 by Lawrence Philips.
 */
namespace Algorithms.StringDistance
{
	using System;
	using System.Linq;
	using System.Text;

	/// <summary>Implements the Double Metaphone phonetic matching algorithm published
	///     by Lawrence Phillips in June 2000 C/C++ Users Journal. 
	/// 
	///     Optimized and ported to C# by Adam Nelson (anelson@nullpointer.net)
	/// 										</summary>
	public class DoubleMetaphone
	{
		#region Fields

		public const int METAPHONE_KEY_LENGTH = 4; //The length of the metaphone keys produced.  4 is sweet spot

		///StringBuilders used to construct the keys
		private readonly StringBuilder m_alternateKey;

		///StringBuilders used to construct the keys
		private readonly StringBuilder m_primaryKey;

		///Variables to track the key length w/o having to grab the .Length attr
		private int m_alternateKeyLength;

		///Actual keys, populated after construction
		private String m_alternateKeyString;

		///Flag indicating if an alternate metaphone key was computed for the word
		private bool m_hasAlternate;

		///Length and last valid zero-based index into word
		private int m_last;

		///Length and last valid zero-based index into word
		private int m_length;

		///Working copy of the word, and the original word
		private String m_originalWord;

		///Variables to track the key length w/o having to grab the .Length attr
		private int m_primaryKeyLength;

		///Actual keys, populated after construction
		private String m_primaryKeyString;

		///Working copy of the word, and the original word
		private String m_word;

		#endregion Fields

		#region Constructors

		/// <summary>Default ctor, initializes by computing the keys of an empty string,
		///     which are both empty strings</summary>
		public DoubleMetaphone()
		{
			//Leave room at the end for writing a bit beyond the length; keys are chopped at the end anyway
			this.m_primaryKey = new StringBuilder(METAPHONE_KEY_LENGTH + 2);
			this.m_alternateKey = new StringBuilder(METAPHONE_KEY_LENGTH + 2);

			this.computeKeys("");
		}

		/// <summary>Constructs a new DoubleMetaphoneDistance object, and initializes it with
		///     the metaphone keys for a given word</summary>
		/// 
		/// <param name="word">Word with which to initialize the object.  Computes the metaphone keys
		///     of this word.</param>
		public DoubleMetaphone(String word)
		{
			//Leave room at the end for writing a bit beyond the length; keys are chopped at the end anyway
			this.m_primaryKey = new StringBuilder(METAPHONE_KEY_LENGTH + 2);
			this.m_alternateKey = new StringBuilder(METAPHONE_KEY_LENGTH + 2);

			this.computeKeys(word);
		}

		#endregion Constructors

		#region Properties

		/// <summary>The alternate metaphone key for the current word, or null if the current
		///     word does not have an alternate key by Double Metaphone</summary>
		public String AlternateKey
		{
			get
			{
				return this.m_hasAlternate ? this.m_alternateKeyString : null;
			}
		}

		/// <summary>The primary metaphone key for the current word</summary>
		public String PrimaryKey
		{
			get
			{
				return this.m_primaryKeyString;
			}
		}

		/// <summary>Original word for which the keys were computed</summary>
		public String Word
		{
			get
			{
				return this.m_originalWord;
			}
		}

		#endregion Properties

		#region Methods

		/// <summary>Static wrapper around the class, enables computation of metaphone keys
		///     without instantiating a class.</summary>
		/// 
		/// <param name="word">Word whose metaphone keys are to be computed</param>
		/// <param name="primaryKey">Ref to var to receive primary metaphone key</param>
		/// <param name="alternateKey">Ref to var to receive alternate metaphone key, or be set to null if
		///     word has no alternate key by double metaphone</param>
		public static void doubleMetaphone(String word, out String primaryKey, out String alternateKey)
		{
			var mp = new DoubleMetaphone(word);

			primaryKey = mp.PrimaryKey;
			alternateKey = mp.AlternateKey;
		}

		/// <summary>Sets a new current word for the instance, computing the new word's metaphone
		///     keys</summary>
		/// 
		/// <param name="word">New word to set to current word.  Discards previous metaphone keys,
		///     and computes new keys for this word</param>
		public void computeKeys(String word)
		{
			this.m_primaryKey.Length = 0;
			this.m_alternateKey.Length = 0;

			this.m_primaryKeyString = "";
			this.m_alternateKeyString = "";

			this.m_primaryKeyLength = this.m_alternateKeyLength = 0;

			this.m_hasAlternate = false;

			this.m_originalWord = word;

			//Copy word to an internal working buffer so it can be modified
			this.m_word = word;

			this.m_length = this.m_word.Length;

			//Compute last valid index into word
			this.m_last = this.m_length - 1;

			//Padd with four spaces, so word can be over-indexed without fear of exception
			this.m_word = String.Concat(this.m_word, "     ");

			//Convert to upper case, since metaphone is not case sensitive
			this.m_word = this.m_word.ToUpper();

			//Now build the keys
			this.buildMetaphoneKeys();
		}

		/**
       * Internal impl of double metaphone algorithm.  Populates m_primaryKey and m_alternateKey.  Modified copy-past of
       * Phillips' original code
       */
		/**
       * Appends the given metaphone character to the primary and alternate keys
       *
       * @param primaryCharacter
       *               Character to append
       */
		/**
       * Appends a metaphone character to the primary, and a possibly different alternate,
       * metaphone keys for the word.
       *
       * @param primaryCharacter
       *               Primary character to append to primary key, and, if no alternate char is present,
       *               the alternate key as well
       * @param alternateCharacter
       *               Alternate character to append to alternate key.  May be null or a zero-length string,
       *               in which case the primary character will be appended to the alternate key instead
       */
		private void addMetaphoneCharacter(String primaryCharacter, String alternateCharacter = null)
		{
			//Is the primary character valid?
			if (primaryCharacter.Length > 0)
			{
				int idx = 0;
				while (idx < primaryCharacter.Length)
				{
					this.m_primaryKey.Length++;
					this.m_primaryKey[this.m_primaryKeyLength++] = primaryCharacter[idx++];
				}
			}

			//Is the alternate character valid?
			if (alternateCharacter != null)
			{
				//Alternate character was provided.  If it is not zero-length, append it, else
				//append the primary string as long as it wasn't zero length and isn't a space character
				if (alternateCharacter.Length > 0)
				{
					this.m_hasAlternate = true;
					if (alternateCharacter[0] != ' ')
					{
						int idx = 0;
						while (idx < alternateCharacter.Length)
						{
							this.m_alternateKey.Length++;
							this.m_alternateKey[this.m_alternateKeyLength++] = alternateCharacter[idx++];
						}
					}
				}
				else
				{
					//No, but if the primary character is valid, add that instead
					if (primaryCharacter.Length > 0 && (primaryCharacter[0] != ' '))
					{
						int idx = 0;
						while (idx < primaryCharacter.Length)
						{
							this.m_alternateKey.Length++;
							this.m_alternateKey[this.m_alternateKeyLength++] = primaryCharacter[idx++];
						}
					}
				}
			}
			else if (primaryCharacter.Length > 0)
			{
				//Else, no alternate character was passed, but a primary was, so append the primary character to the alternate key
				int idx = 0;
				while (idx < primaryCharacter.Length)
				{
					this.m_alternateKey.Length++;
					this.m_alternateKey[this.m_alternateKeyLength++] = primaryCharacter[idx++];
				}
			}
		}

		/**
       * Tests if any of the strings passed as variable arguments are at the given start position and
       * length within word
       *
       * @param start   Start position in m_word
       * @param length  Length of substring starting at start in m_word to compare to the given strings
       * @param strings params array of zero or more strings for which to search in m_word
       *
       * @return true if any one string in the strings array was found in m_word at the given position
       *         and length
       */
		private bool areStringsAt(int start, int length, params String[] strings)
		{
			if (start < 0)
			{
				//Sometimes, as a result of expressions like "current - 2" for start,
				//start ends up negative.  Since no string can be present at a negative offset, this is always false
				return false;
			}

			String target = this.m_word.Substring(start, length);

			return strings.Any(t => t == target);
		}

		private void buildMetaphoneKeys()
		{
			int current = 0;
			if (this.m_length < 1)
			{
				return;
			}

			//skip these when at start of word
			if (this.areStringsAt(0, 2, "GN", "KN", "PN", "WR", "PS"))
			{
				current += 1;
			}

			//Initial 'X' is pronounced 'Z' e.g. 'Xavier'
			if (this.m_word[0] == 'X')
			{
				this.addMetaphoneCharacter("S"); //'Z' maps to 'S'
				current += 1;
			}

			///////////main loop//////////////////////////
			while ((this.m_primaryKeyLength < METAPHONE_KEY_LENGTH)
					 || (this.m_alternateKeyLength < METAPHONE_KEY_LENGTH))
			{
				if (current >= this.m_length)
				{
					break;
				}

				switch (this.m_word[current])
				{
					case 'A':
					case 'E':
					case 'I':
					case 'O':
					case 'U':
					case 'Y':
						if (current == 0)
						{
							//all init vowels now map to 'A'
							this.addMetaphoneCharacter("A");
						}
						current += 1;
						break;

					case 'B':

						//"-mb", e.g", "dumb", already skipped over...
						this.addMetaphoneCharacter("P");

						if (this.m_word[current + 1] == 'B')
						{
							current += 2;
						}
						else
						{
							current += 1;
						}
						break;

					case 'Ç':
						this.addMetaphoneCharacter("S");
						current += 1;
						break;

					case 'C':
						//various germanic
						if ((current > 1) && !this.isVowel(current - 2) && this.areStringsAt((current - 1), 3, "ACH")
							 &&
							 ((this.m_word[current + 2] != 'I')
							  &&
							  ((this.m_word[current + 2] != 'E')
								|| this.areStringsAt((current - 2), 6, "BACHER", "MACHER"))))
						{
							this.addMetaphoneCharacter("K");
							current += 2;
							break;
						}

						//special case 'caesar'
						if ((current == 0) && this.areStringsAt(current, 6, "CAESAR"))
						{
							this.addMetaphoneCharacter("S");
							current += 2;
							break;
						}

						//italian 'chianti'
						if (this.areStringsAt(current, 4, "CHIA"))
						{
							this.addMetaphoneCharacter("K");
							current += 2;
							break;
						}

						if (this.areStringsAt(current, 2, "CH"))
						{
							//find 'michael'
							if ((current > 0) && this.areStringsAt(current, 4, "CHAE"))
							{
								this.addMetaphoneCharacter("K", "X");
								current += 2;
								break;
							}

							//greek roots e.g. 'chemistry', 'chorus'
							if ((current == 0)
								 &&
								 (this.areStringsAt((current + 1), 5, "HARAC", "HARIS")
								  || this.areStringsAt((current + 1), 3, "HOR", "HYM", "HIA", "HEM"))
								 && !this.areStringsAt(0, 5, "CHORE"))
							{
								this.addMetaphoneCharacter("K");
								current += 2;
								break;
							}

							//germanic, greek, or otherwise 'ch' for 'kh' sound
							if ((this.areStringsAt(0, 4, "VAN ", "VON ") || this.areStringsAt(0, 3, "SCH"))
								 // 'architect but not 'arch', 'orchestra', 'orchid'
								 || this.areStringsAt((current - 2), 6, "ORCHES", "ARCHIT", "ORCHID")
								 || this.areStringsAt((current + 2), 1, "T", "S")
								 ||
								 ((this.areStringsAt((current - 1), 1, "A", "O", "U", "E") || (current == 0))
								  //e.g., 'wachtler', 'wechsler', but not 'tichner'
								  &&
								  this.areStringsAt((current + 2), 1, "L", "R", "N", "M", "B", "H", "F", "V", "W", " ")))
							{
								this.addMetaphoneCharacter("K");
							}
							else
							{
								if (current > 0)
								{
									if (this.areStringsAt(0, 2, "MC"))
									{
										//e.g., "McHugh"
										this.addMetaphoneCharacter("K");
									}
									else
									{
										this.addMetaphoneCharacter("X", "K");
									}
								}
								else
								{
									this.addMetaphoneCharacter("X");
								}
							}
							current += 2;
							break;
						}
						//e.g, 'czerny'
						if (this.areStringsAt(current, 2, "CZ") && !this.areStringsAt((current - 2), 4, "WICZ"))
						{
							this.addMetaphoneCharacter("S", "X");
							current += 2;
							break;
						}

						//e.g., 'focaccia'
						if (this.areStringsAt((current + 1), 3, "CIA"))
						{
							this.addMetaphoneCharacter("X");
							current += 3;
							break;
						}

						//double 'C', but not if e.g. 'McClellan'
						if (this.areStringsAt(current, 2, "CC") && !((current == 1) && (this.m_word[0] == 'M')))
						{
							//'bellocchio' but not 'bacchus'
							if (this.areStringsAt((current + 2), 1, "I", "E", "H")
								 && !this.areStringsAt((current + 2), 2, "HU"))
							{
								//'accident', 'accede' 'succeed'
								if (((current == 1) && (this.m_word[current - 1] == 'A'))
									 || this.areStringsAt((current - 1), 5, "UCCEE", "UCCES"))
								{
									this.addMetaphoneCharacter("KS");
								}
								//'bacci', 'bertucci', other italian
								else
								{
									this.addMetaphoneCharacter("X");
								}
								current += 3;
								break;
							}

							//Pierce's rule
							this.addMetaphoneCharacter("K");
							current += 2;
							break;
						}

						if (this.areStringsAt(current, 2, "CK", "CG", "CQ"))
						{
							this.addMetaphoneCharacter("K");
							current += 2;
							break;
						}

						if (this.areStringsAt(current, 2, "CI", "CE", "CY"))
						{
							//italian vs. english
							if (this.areStringsAt(current, 3, "CIO", "CIE", "CIA"))
							{
								this.addMetaphoneCharacter("S", "X");
							}
							else
							{
								this.addMetaphoneCharacter("S");
							}
							current += 2;
							break;
						}

						//else
						this.addMetaphoneCharacter("K");

						//name sent in 'mac caffrey', 'mac gregor
						if (this.areStringsAt((current + 1), 2, " C", " Q", " G"))
						{
							current += 3;
						}
						else if (this.areStringsAt((current + 1), 1, "C", "K", "Q")
									&& !this.areStringsAt((current + 1), 2, "CE", "CI"))
						{
							current += 2;
						}
						else
						{
							current += 1;
						}
						break;

					case 'D':
						if (this.areStringsAt(current, 2, "DG"))
						{
							if (this.areStringsAt((current + 2), 1, "I", "E", "Y"))
							{
								//e.g. 'edge'
								this.addMetaphoneCharacter("J");
								current += 3;
								break;
							}

							//e.g. 'edgar'
							this.addMetaphoneCharacter("TK");
							current += 2;
							break;
						}

						if (this.areStringsAt(current, 2, "DT", "DD"))
						{
							this.addMetaphoneCharacter("T");
							current += 2;
							break;
						}

						//else
						this.addMetaphoneCharacter("T");
						current += 1;
						break;

					case 'F':
						if (this.m_word[current + 1] == 'F')
						{
							current += 2;
						}
						else
						{
							current += 1;
						}
						this.addMetaphoneCharacter("F");
						break;

					case 'G':
						if (this.m_word[current + 1] == 'H')
						{
							if ((current > 0) && !this.isVowel(current - 1))
							{
								this.addMetaphoneCharacter("K");
								current += 2;
								break;
							}

							if (current < 3)
							{
								//'ghislane', ghiradelli
								if (current == 0)
								{
									this.addMetaphoneCharacter(this.m_word[current + 2] == 'I' ? "J" : "K");
									current += 2;
									break;
								}
							}
							//Parker's rule (with some further refinements) - e.g., 'hugh'
							if (((current > 1) && this.areStringsAt((current - 2), 1, "B", "H", "D")) //e.g., 'bough'
								 || ((current > 2) && this.areStringsAt((current - 3), 1, "B", "H", "D"))
								 //e.g., 'broughton'
								 || ((current > 3) && this.areStringsAt((current - 4), 1, "B", "H")))
							{
								current += 2;
								break;
							}

							//e.g., 'laugh', 'McLaughlin', 'cough', 'gough', 'rough', 'tough'
							if ((current > 2) && (this.m_word[current - 1] == 'U')
								 && this.areStringsAt((current - 3), 1, "C", "G", "L", "R", "T"))
							{
								this.addMetaphoneCharacter("F");
							}
							else if ((current > 0) && this.m_word[current - 1] != 'I')
							{
								this.addMetaphoneCharacter("K");
							}

							current += 2;
							break;
						}

						if (this.m_word[current + 1] == 'N')
						{
							if ((current == 1) && this.isVowel(0) && !this.isWordSlavoGermanic())
							{
								this.addMetaphoneCharacter("KN", "N");
							}
							else
								 //not e.g. 'cagney'
								 if (!this.areStringsAt((current + 2), 2, "EY") && (this.m_word[current + 1] != 'Y')
									  && !this.isWordSlavoGermanic())
							{
								this.addMetaphoneCharacter("N", "KN");
							}
							else
							{
								this.addMetaphoneCharacter("KN");
							}
							current += 2;
							break;
						}

						//'tagliaro'
						if (this.areStringsAt((current + 1), 2, "LI") && !this.isWordSlavoGermanic())
						{
							this.addMetaphoneCharacter("KL", "L");
							current += 2;
							break;
						}

						//-ges-,-gep-,-gel-, -gie- at beginning
						if ((current == 0)
							 &&
							 ((this.m_word[current + 1] == 'Y')
							  ||
							  this.areStringsAt(
									(current + 1), 2, "ES", "EP", "EB", "EL", "EY", "IB", "IL", "IN", "IE", "EI", "ER")))
						{
							this.addMetaphoneCharacter("K", "J");
							current += 2;
							break;
						}

						// -ger-,  -gy-
						if ((this.areStringsAt((current + 1), 2, "ER") || (this.m_word[current + 1] == 'Y'))
							 && !this.areStringsAt(0, 6, "DANGER", "RANGER", "MANGER")
							 && !this.areStringsAt((current - 1), 1, "E", "I")
							 && !this.areStringsAt((current - 1), 3, "RGY", "OGY"))
						{
							this.addMetaphoneCharacter("K", "J");
							current += 2;
							break;
						}

						// italian e.g, 'biaggi'
						if (this.areStringsAt((current + 1), 1, "E", "I", "Y")
							 || this.areStringsAt((current - 1), 4, "AGGI", "OGGI"))
						{
							//obvious germanic
							if ((this.areStringsAt(0, 4, "VAN ", "VON ") || this.areStringsAt(0, 3, "SCH"))
								 || this.areStringsAt((current + 1), 2, "ET"))
							{
								this.addMetaphoneCharacter("K");
							}
							else
								 //always soft if french ending
								 if (this.areStringsAt((current + 1), 4, "IER "))
							{
								this.addMetaphoneCharacter("J");
							}
							else
							{
								this.addMetaphoneCharacter("J", "K");
							}
							current += 2;
							break;
						}

						if (this.m_word[current + 1] == 'G')
						{
							current += 2;
						}
						else
						{
							current += 1;
						}
						this.addMetaphoneCharacter("K");
						break;

					case 'H':
						//only keep if first & before vowel or btw. 2 vowels
						if (((current == 0) || this.isVowel(current - 1)) && this.isVowel(current + 1))
						{
							this.addMetaphoneCharacter("H");
							current += 2;
						}
						else //also takes care of 'HH'
						{
							current += 1;
						}
						break;

					case 'J':
						//obvious spanish, 'jose', 'san jacinto'
						if (this.areStringsAt(current, 4, "JOSE") || this.areStringsAt(0, 4, "SAN "))
						{
							if (((current == 0) && (this.m_word[current + 4] == ' ')) || this.areStringsAt(0, 4, "SAN "))
							{
								this.addMetaphoneCharacter("H");
							}
							else
							{
								this.addMetaphoneCharacter("J", "H");
							}
							current += 1;
							break;
						}

						if ((current == 0) && !this.areStringsAt(current, 4, "JOSE"))
						{
							this.addMetaphoneCharacter("J", "A"); //Yankelovich/Jankelowicz
						}
						else
							 //spanish pron. of e.g. 'bajador'
							 if (this.isVowel(current - 1) && !this.isWordSlavoGermanic()
								  && ((this.m_word[current + 1] == 'A') || (this.m_word[current + 1] == 'O')))
						{
							this.addMetaphoneCharacter("J", "H");
						}
						else if (current == this.m_last)
						{
							this.addMetaphoneCharacter("J", " ");
						}
						else if (!this.areStringsAt((current + 1), 1, "L", "T", "K", "S", "N", "M", "B", "Z")
									&& !this.areStringsAt((current - 1), 1, "S", "K", "L"))
						{
							this.addMetaphoneCharacter("J");
						}

						if (this.m_word[current + 1] == 'J') //it could happen!
						{
							current += 2;
						}
						else
						{
							current += 1;
						}
						break;

					case 'K':
						if (this.m_word[current + 1] == 'K')
						{
							current += 2;
						}
						else
						{
							current += 1;
						}
						this.addMetaphoneCharacter("K");
						break;

					case 'L':
						if (this.m_word[current + 1] == 'L')
						{
							//spanish e.g. 'cabrillo', 'gallegos'
							if (((current == (this.m_length - 3))
								  && this.areStringsAt((current - 1), 4, "ILLO", "ILLA", "ALLE"))
								 ||
								 ((this.areStringsAt((this.m_last - 1), 2, "AS", "OS")
									|| this.areStringsAt(this.m_last, 1, "A", "O"))
								  && this.areStringsAt((current - 1), 4, "ALLE")))
							{
								this.addMetaphoneCharacter("L", " ");
								current += 2;
								break;
							}
							current += 2;
						}
						else
						{
							current += 1;
						}
						this.addMetaphoneCharacter("L");
						break;

					case 'M':
						if ((this.areStringsAt((current - 1), 3, "UMB")
							  && (((current + 1) == this.m_last) || this.areStringsAt((current + 2), 2, "ER")))
							 //'dumb','thumb'
							 || (this.m_word[current + 1] == 'M'))
						{
							current += 2;
						}
						else
						{
							current += 1;
						}
						this.addMetaphoneCharacter("M");
						break;

					case 'N':
						if (this.m_word[current + 1] == 'N')
						{
							current += 2;
						}
						else
						{
							current += 1;
						}
						this.addMetaphoneCharacter("N");
						break;

					case 'Ñ':
						current += 1;
						this.addMetaphoneCharacter("N");
						break;

					case 'P':
						if (this.m_word[current + 1] == 'H')
						{
							this.addMetaphoneCharacter("F");
							current += 2;
							break;
						}

						//also account for "campbell", "raspberry"
						if (this.areStringsAt((current + 1), 1, "P", "B"))
						{
							current += 2;
						}
						else
						{
							current += 1;
						}
						this.addMetaphoneCharacter("P");
						break;

					case 'Q':
						if (this.m_word[current + 1] == 'Q')
						{
							current += 2;
						}
						else
						{
							current += 1;
						}
						this.addMetaphoneCharacter("K");
						break;

					case 'R':
						//french e.g. 'rogier', but exclude 'hochmeier'
						if ((current == this.m_last) && !this.isWordSlavoGermanic()
							 && this.areStringsAt((current - 2), 2, "IE")
							 && !this.areStringsAt((current - 4), 2, "ME", "MA"))
						{
							this.addMetaphoneCharacter("", "R");
						}
						else
						{
							this.addMetaphoneCharacter("R");
						}

						if (this.m_word[current + 1] == 'R')
						{
							current += 2;
						}
						else
						{
							current += 1;
						}
						break;

					case 'S':
						//special cases 'island', 'isle', 'carlisle', 'carlysle'
						if (this.areStringsAt((current - 1), 3, "ISL", "YSL"))
						{
							current += 1;
							break;
						}

						//special case 'sugar-'
						if ((current == 0) && this.areStringsAt(current, 5, "SUGAR"))
						{
							this.addMetaphoneCharacter("X", "S");
							current += 1;
							break;
						}

						if (this.areStringsAt(current, 2, "SH"))
						{
							//germanic
							this.addMetaphoneCharacter(
								 this.areStringsAt((current + 1), 4, "HEIM", "HOEK", "HOLM", "HOLZ") ? "S" : "X");
							current += 2;
							break;
						}

						//italian & armenian
						if (this.areStringsAt(current, 3, "SIO", "SIA") || this.areStringsAt(current, 4, "SIAN"))
						{
							if (!this.isWordSlavoGermanic())
							{
								this.addMetaphoneCharacter("S", "X");
							}
							else
							{
								this.addMetaphoneCharacter("S");
							}
							current += 3;
							break;
						}

						//german & anglicisations, e.g. 'smith' match 'schmidt', 'snider' match 'schneider'
						//also, -sz- in slavic language altho in hungarian it is pronounced 's'
						if (((current == 0) && this.areStringsAt((current + 1), 1, "M", "N", "L", "W"))
							 || this.areStringsAt((current + 1), 1, "Z"))
						{
							this.addMetaphoneCharacter("S", "X");
							if (this.areStringsAt((current + 1), 1, "Z"))
							{
								current += 2;
							}
							else
							{
								current += 1;
							}
							break;
						}

						if (this.areStringsAt(current, 2, "SC"))
						{
							//Schlesinger's rule
							if (this.m_word[current + 2] == 'H')
							{
								//dutch origin, e.g. 'school', 'schooner'
								if (this.areStringsAt((current + 3), 2, "OO", "ER", "EN", "UY", "ED", "EM"))
								{
									//'schermerhorn', 'schenker'
									if (this.areStringsAt((current + 3), 2, "ER", "EN"))
									{
										this.addMetaphoneCharacter("X", "SK");
									}
									else
									{
										this.addMetaphoneCharacter("SK");
									}
									current += 3;
									break;
								}

								if ((current == 0) && !this.isVowel(3) && (this.m_word[3] != 'W'))
								{
									this.addMetaphoneCharacter("X", "S");
								}
								else
								{
									this.addMetaphoneCharacter("X");
								}
								current += 3;
								break;
							}

							if (this.areStringsAt((current + 2), 1, "I", "E", "Y"))
							{
								this.addMetaphoneCharacter("S");
								current += 3;
								break;
							}
							//else
							this.addMetaphoneCharacter("SK");
							current += 3;
							break;
						}

						//french e.g. 'resnais', 'artois'
						if ((current == this.m_last) && this.areStringsAt((current - 2), 2, "AI", "OI"))
						{
							this.addMetaphoneCharacter("", "S");
						}
						else
						{
							this.addMetaphoneCharacter("S");
						}

						if (this.areStringsAt((current + 1), 1, "S", "Z"))
						{
							current += 2;
						}
						else
						{
							current += 1;
						}
						break;

					case 'T':
						if (this.areStringsAt(current, 4, "TION"))
						{
							this.addMetaphoneCharacter("X");
							current += 3;
							break;
						}

						if (this.areStringsAt(current, 3, "TIA", "TCH"))
						{
							this.addMetaphoneCharacter("X");
							current += 3;
							break;
						}

						if (this.areStringsAt(current, 2, "TH") || this.areStringsAt(current, 3, "TTH"))
						{
							//special case 'thomas', 'thames' or germanic
							if (this.areStringsAt((current + 2), 2, "OM", "AM")
								 || this.areStringsAt(0, 4, "VAN ", "VON ") || this.areStringsAt(0, 3, "SCH"))
							{
								this.addMetaphoneCharacter("T");
							}
							else
							{
								this.addMetaphoneCharacter("0", "T");
							}
							current += 2;
							break;
						}

						if (this.areStringsAt((current + 1), 1, "T", "D"))
						{
							current += 2;
						}
						else
						{
							current += 1;
						}
						this.addMetaphoneCharacter("T");
						break;

					case 'V':
						if (this.m_word[current + 1] == 'V')
						{
							current += 2;
						}
						else
						{
							current += 1;
						}
						this.addMetaphoneCharacter("F");
						break;

					case 'W':
						//can also be in middle of word
						if (this.areStringsAt(current, 2, "WR"))
						{
							this.addMetaphoneCharacter("R");
							current += 2;
							break;
						}

						if ((current == 0) && (this.isVowel(current + 1) || this.areStringsAt(current, 2, "WH")))
						{
							//Wasserman should match Vasserman
							if (this.isVowel(current + 1))
							{
								this.addMetaphoneCharacter("A", "F");
							}
							else
							{
								//need Uomo to match Womo
								this.addMetaphoneCharacter("A");
							}
						}

						//Arnow should match Arnoff
						if (((current == this.m_last) && this.isVowel(current - 1))
							 || this.areStringsAt((current - 1), 5, "EWSKI", "EWSKY", "OWSKI", "OWSKY")
							 || this.areStringsAt(0, 3, "SCH"))
						{
							this.addMetaphoneCharacter("", "F");
							current += 1;
							break;
						}

						//polish e.g. 'filipowicz'
						if (this.areStringsAt(current, 4, "WICZ", "WITZ"))
						{
							this.addMetaphoneCharacter("TS", "FX");
							current += 4;
							break;
						}

						//else skip it
						current += 1;
						break;

					case 'X':
						//french e.g. breaux
						if (
							 !((current == this.m_last)
								&&
								(this.areStringsAt((current - 3), 3, "IAU", "EAU")
								 || this.areStringsAt((current - 2), 2, "AU", "OU"))))
						{
							this.addMetaphoneCharacter("KS");
						}

						if (this.areStringsAt((current + 1), 1, "C", "X"))
						{
							current += 2;
						}
						else
						{
							current += 1;
						}
						break;

					case 'Z':
						//chinese pinyin e.g. 'zhao'
						if (this.m_word[current + 1] == 'H')
						{
							this.addMetaphoneCharacter("J");
							current += 2;
							break;
						}

						if (this.areStringsAt((current + 1), 2, "ZO", "ZI", "ZA")
							 || (this.isWordSlavoGermanic() && ((current > 0) && this.m_word[current - 1] != 'T')))
						{
							this.addMetaphoneCharacter("S", "TS");
						}
						else
						{
							this.addMetaphoneCharacter("S");
						}

						if (this.m_word[current + 1] == 'Z')
						{
							current += 2;
						}
						else
						{
							current += 1;
						}
						break;

					default:
						current += 1;
						break;
				}
			}

			//Finally, chop off the keys at the proscribed length
			if (this.m_primaryKeyLength > METAPHONE_KEY_LENGTH)
			{
				this.m_primaryKey.Length = METAPHONE_KEY_LENGTH;
			}

			if (this.m_alternateKeyLength > METAPHONE_KEY_LENGTH)
			{
				this.m_alternateKey.Length = METAPHONE_KEY_LENGTH;
			}

			this.m_primaryKeyString = this.m_primaryKey.ToString();
			this.m_alternateKeyString = this.m_alternateKey.ToString();
		}

		private bool isVowel(int pos)
		{
			if ((pos < 0) || (pos >= this.m_length))
			{
				return false;
			}

			Char it = this.m_word[pos];

			if ((it == 'E') || (it == 'A') || (it == 'I') || (it == 'O') || (it == 'U') || (it == 'Y'))
			{
				return true;
			}

			return false;
		}

		private bool isWordSlavoGermanic()
		{
			return (this.m_word.IndexOf("W", StringComparison.Ordinal) != -1)
					 || (this.m_word.IndexOf("K", StringComparison.Ordinal) != -1)
					 || (this.m_word.IndexOf("CZ", StringComparison.Ordinal) != -1)
					 || (this.m_word.IndexOf("WITZ", StringComparison.Ordinal) != -1);
		}

		#endregion Methods
	}
}