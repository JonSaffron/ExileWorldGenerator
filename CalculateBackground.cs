using System;
using System.Collections.Generic;

namespace ExileMappedBackground
    {
    class CalculateBackground
        {
        private static readonly byte[] MapData = BuildMapData();
        private static readonly Dictionary<int, byte[]> BackgroundObjectsXLookup = BuildBackgroundObjectsXLookup();
        private static readonly byte[] LookupForUnmatchedHash = BuildLookupForUnmatchedHash();
        private static readonly Dictionary<int, byte[]> BackgroundObjectsHandlerLookup = BuildBackgroundObjectsHandlerLookup();
        private static readonly byte[] BackgroundLookup = BuildBackgroundLookupTable();

        public MapResult GetBackground(byte squareX, byte squareY)
            {
            byte accumulator;
            byte xreg;
            byte yreg;
            Flags f = new Flags();
            MapResult result = new MapResult();

            accumulator = squareY;                              // LDA square_y
            xreg = accumulator;                                 // TAX
            LogicalShiftRight(ref accumulator, ref f);       // LSR A
            accumulator = (byte) (accumulator ^ squareX);       // EOR square_x
            accumulator = (byte) (accumulator & 0xF8);          // AND #$F8
            LogicalShiftRight(ref accumulator, ref f);       // LSR A
            AddWithCarry(ref accumulator, squareX, ref f);  // ADC square_x
            LogicalShiftRight(ref accumulator, ref f);       // LSR A
            AddWithCarry(ref accumulator, squareY, ref f);  // ADC square_y
            byte zp_various_9d = accumulator;                   // zp_various_9d    ; f_xy is a function of square_x and square_y
            accumulator = xreg;                                 // TXA              ; A = square_y
            Compare(accumulator, 0x79, ref f);// CMP #$79
            if (!f.Carry) goto L17A8;                           // BCC L17A8        ; taken if square_y<0x79
            Compare(accumulator, 0xBF, ref f);// CMP #$BF
            if (!f.Carry) goto not_mapped2;                     // BCC not_mapped2  ; taken if square_y<0xbf

            // before: square_y = 0xc0 ... 0xff
            // after:  square_y = 0x7a ... 0xb9
            SubtractWithBorrow(ref accumulator, 0x46, ref f);   // SBC #$46         ; y >= &c0 ; y -= &46;
    
L17A8:
            // A = square_y
            Compare(accumulator, 0x48, ref f);// CMP #$48
            if (f.Carry) goto L17B2;                              // BCS L17B2        ; taken if square_y>=0x48
            Compare(accumulator, 0x3E, ref f);// CMP #$3E
            if (f.Carry) goto not_mapped2;                        // BCS not_mapped2  ; taken if square_y>=0x3e

            // before: square_y = 0x00 ... 0x3d
            // after:  square_y = 0x0a ... 0x47
            AddWithCarry(ref accumulator, 0x0A, ref f);     // ADC #$0A
    
L17B2:
            byte f2_xy = accumulator;                           // STA f2_xy        ; at this point, a function of square_y
            accumulator = (byte) (accumulator & 0b10101000);    // AND #%10101000                   
            accumulator = (byte) (accumulator ^ 0b01101111);    // EOR #%01101111
            LogicalShiftRight(ref accumulator, ref f);   // LSR A
            AddWithCarry(ref accumulator, squareX, ref f);      // ADC square_x
            accumulator = (byte) (accumulator ^ 0b01100000);    // EOR #%01100000
            AddWithCarry(ref accumulator, 0b00101000, ref f);   // ADC #%00101000
            byte f3_xy = accumulator;                           // STA f3_xy        ; f3_xy is another function of square_x and square_y
            accumulator = (byte) (accumulator & 0b00111000);    // AND #%00111000
            accumulator = (byte) (accumulator ^ 0b10100100);    // EOR #%10100100
            AddWithCarry(ref accumulator, f2_xy, ref f);        // ADC f2_xy
            f2_xy = accumulator;                                // STA f2_xy        ; f2_xy a function of square_x and square_y
            yreg = accumulator;                                 // TAY              ; A is f2_xy
            accumulator = (byte) (accumulator ^ 0b00101100);    // EOR #%00101100
            AddWithCarry(ref accumulator, f3_xy, ref f);     // ADC f3_xy      ; A is a function of f2_xy and f3_xy
            Compare(yreg, 0x20, ref f);        // CPY #$20
            if (f.Carry) goto not_mapped2;                        // BCS not_mapped2        ; taken if f2_xy>=0x20
            Compare(accumulator, 0x20, ref f); // CMP #$20
            if (f.Carry) goto not_mapped;                         // BCS not_mapped         ; if A >= &20, don't use mapped data

            result.IsMappedData = true;                        // DEC square_is_mapped_data
            yreg = accumulator;                                 // TAY                    ; Y = A = f(f2_xy,f3_xy)
            ArithmeticShiftLeft(ref accumulator, ref f);    // ASL A
            ArithmeticShiftLeft(ref accumulator, ref f);    // ASL A
            ArithmeticShiftLeft(ref accumulator, ref f);    // ASL A
            accumulator = (byte) (accumulator ^ f2_xy);         // EOR f2_xy
            byte map_address = accumulator;                     // STA map_address        ; &a4 = (A * 8) ^ &10;
    
            accumulator = yreg;                                 // TYA                    ; A = Y = f(f2_xy,f3_xy)
            accumulator = (byte) (accumulator & 0x03);          // AND #$03
            AddWithCarry(ref accumulator, 0, ref f);    // ADC #HI(map_data)
            byte map_address_high = accumulator;                // STA map_address_high        ; a5 = (A & 3) + &4f;
    
            yreg = 0;                                           // LDY #LO(map_data)           ; $EC ; &4fec - &53ec
            result.PositionInMappedData = (map_address_high << 8) + map_address;
            result.Result = MapData[result.PositionInMappedData + yreg];            // LDA (map_address),Y         ; use mapped data
            return result;                                      // RTS

L17EC:
            goto L1937;                                         // JMP L1937
           
via_return_background_empty:
            goto return_background_empty;                       // JMP return_background_empty
    
//  If not mapped data, are we on or above the surface?
not_mapped:
// A = 
            Compare(accumulator, 0x3d, ref f);                        // CMP #$3D
            if (!f.Carry) goto via_return_background_empty;       // BCC via_return_background_empty
not_mapped2:
            Compare(xreg, 0x4e, ref f);  // CPX #$4E ; if square_y < &4e, return empty space
            if (!f.Carry) goto via_return_background_empty;       // BCC via_return_background_empty
            if (f.Zero) goto L17EC;                               // BEQ L17EC ; if square_y = &4e, return bushes
            Compare(xreg, 0x4f, ref f);    // CPX #$4F
            if (!f.Zero) goto below_surface;                      // BNE below_surface
            accumulator = squareX;                              // LDA square_x ; if square_y = &4f, do surface
            if (accumulator == 0x40)                            // CMP #$40
                goto return_background_grass_frond;             // BEQ return_background_grass_frond ; force (&40, &4f) to be a grass frond
            yreg = 1;                                           // LDY #$01
            goto via_background_is_114f_lookup_with_y;          // JMP via_background_is_114f_lookup_with_y        ; otherwise, return wall

return_background_grass_frond:
            result.Result = 0x62;                               // LDA #$62 ; &62 = grass frond
            return result;                                       // RTS


//  Things get rather hairy below the surface...
below_surface:
            yreg = zp_various_9d;                               // LDY zp_various_9d ; f_xy
            accumulator = 0;                                    // LDA #$00
            zp_various_9d = accumulator;                        // STA zp_various_9d
            accumulator = squareX;                              // LDA square_x
            BitTest(accumulator, squareY, ref f);        // BIT square_y
            if (f.Negative) goto L1821;                         // BMI L1821
            AddWithCarry(ref accumulator, 0x1d, ref f);     // ADC #$1D
            Compare(accumulator, 0x5e, ref f); // CMP #$5E
            goto L1825;                                          // JMP L1825

L1821:
            AddWithCarry(ref accumulator, 0x07, ref f);     // ADC #$07
            Compare(accumulator, 0x2b, ref f); //CMP #$2B

L1825:
            if (!f.Carry) goto L187C;                           // BCC L187C
            accumulator = yreg;                                 // TYA
            And(ref accumulator, 0xE8, ref f);           // AND #$E8
            Compare(accumulator, squareY, ref f);   // CMP square_y
            if (!f.Carry) goto L187C;                           // BCC L187C
            zp_various_9d = yreg;                               // STY zp_various_9d
            accumulator = xreg;                                 // TXA
            ArithmeticShiftLeft(ref accumulator, ref f);        // ASL A
            AddWithCarry(ref accumulator, squareY, ref f);  // ADC square_y
            LogicalShiftRight(ref accumulator, ref f);          // LSR A
            AddWithCarry(ref accumulator, squareY, ref f);  // ADC square_y
            And(ref accumulator, 0xE0, ref f);            // AND #$E0
            AddWithCarry(ref accumulator, squareX, ref f);  // ADC square_x
            And(ref accumulator, 0xE8, ref f);            // AND #$E8
            if (accumulator != 0) goto no_mushrooms;            // BNE no_mushrooms
            accumulator = squareY;                              // LDA square_y
            if ((accumulator & 0x80) == 0) goto via_return_background_empty;  // BPL via_return_background_empty ; no mushrooms if square_y < &80
            accumulator = squareX;                              // LDA square_x
            LogicalShiftRight(ref accumulator, ref f);          // LSR A
            LogicalShiftRight(ref accumulator, ref f);          // LSR A
            LogicalShiftRight(ref accumulator, ref f);          // LSR A
            xreg = accumulator;                                 // TAX
            

return_background_mushrooms:
            accumulator = 0x0E;                                 // LDA #$0E ; &0e = mushrooms (on floor)
            Compare(xreg, 0x0A, ref f);       // CPX #$0A
            if (!f.Zero) goto L1851;                            // BNE L1851
            accumulator = 0x8E;                                 // LDA #$8E ; &8e = mushrooms (on ceiling)

L1851:
            result.Result = accumulator;
            return result;                                      // RTS

no_mushrooms:
            accumulator = yreg;                                 // TYA ; Y = f_xy
            LogicalShiftRight(ref accumulator, ref f);          // LSR A
            LogicalShiftRight(ref accumulator, ref f);          // LSR A
            And(ref accumulator, 0x30, ref f);           // AND #$30
            LogicalShiftRight(ref accumulator, ref f);          // LSR A
            AddWithCarry(ref accumulator, squareX, ref f);  // ADC square_x
            LogicalShiftRight(ref accumulator, ref f);          // LSR A
            accumulator ^= squareX;                             // EOR square_x
            LogicalShiftRight(ref accumulator, ref f);          // LSR A
            accumulator ^= squareX;                             // EOR square_x
            AddWithCarry(ref accumulator, squareX, ref f);  // ADC square_x
            And(ref accumulator, 0xFD, ref f);           // AND #$FD
            accumulator ^= squareX;                             // EOR square_x
            And(ref accumulator, 0x07, ref f);           // AND #$07
            if (!f.Zero) goto L1878;                            // BNE L1878
            accumulator = squareX;                              // LDA square_x
            if (f.Negative) goto L1875;                         // BMI L1875
            LogicalShiftRight(ref accumulator, ref f);          // LSR A
            AddWithCarry(ref accumulator, squareY, ref f);  // ADC square_y
            And(ref accumulator, 0x30, ref f);            // AND #$30
            if (f.Zero) goto L1878;                             // BEQ L1878

L1875:
            result.Result = 0x8;                                  // LDA #$08 ; return hash point 8
            return result;                                        // RTS

L1878:
            Compare(xreg, 0x52, ref f);       // CPX #$52
            if (f.Carry) goto L187F;                            // BCS L187F

L187C:
            goto background_is_114f_lookup_with_top_of_9d;      // JMP background_is_114f_lookup_with_top_of_9d

L187F:
            accumulator = yreg;                                 // TYA
            And(ref accumulator, 0x68, ref f);           // AND #$68
            AddWithCarry(ref accumulator, squareY, ref f); // ADC square_y
            LogicalShiftRight(ref accumulator, ref f);          // LSR A
            AddWithCarry(ref accumulator, squareY, ref f);  // ADC square_y
            LogicalShiftRight(ref accumulator, ref f);          // LSR A
            AddWithCarry(ref accumulator, squareY, ref f);  // ADC square_y
            And(ref accumulator, 0xFC, ref f);          // AND #$FC
            accumulator ^= squareY;                            // EOR square_y
            And(ref accumulator, 0x17, ref f);          // AND #$17
            if (!f.Zero) goto L18DF;                            // BNE L18DF
            accumulator = yreg;                                 // TYA
            AddWithCarry(ref accumulator, squareX, ref f);  // ADC square_x
            And(ref accumulator, 0x50, ref f);              // AND #$50
            if (f.Zero) goto return_background_empty;           // BEQ return_background_empty
            And(ref accumulator, squareX, ref f);       // AND square_x
            LogicalShiftRight(ref accumulator, ref f);          // LSR A
            LogicalShiftRight(ref accumulator, ref f);          // LSR A
            AddWithCarry(ref accumulator, squareY, ref f);  // ADC square_y
            LogicalShiftRight(ref accumulator, ref f);          // LSR A
            LogicalShiftRight(ref accumulator, ref f);          // LSR A
            And(ref accumulator, 0x0F, ref f);          // AND #$0F
            Compare(accumulator, 0x08, ref f);  // CMP #$08
            if (!f.Carry) goto L18AF;                            // BCC L18AF
            BitTest(accumulator, zp_various_9d, ref f);     // BIT zp_various_9d
            if (!f.Overflow) goto L18C1;                            // BVC L18C1
            Or(ref accumulator, 0x04, ref f);                       // ORA #$04
            if (!f.Zero) goto L18C1;                                // BNE L18C1

L18AF:
            zp_various_9d = accumulator;                        // STA zp_various_9c
            accumulator ^= 0x5;                                 // EOR #$05
            Compare(accumulator, 0x06, ref f);  // CMP #$06
            accumulator = zp_various_9d;                        // LDA zp_various_9c
            if (f.Carry) goto L18C1;                            // BCS L18C1
            accumulator = yreg;                                 // TYA
            LogicalShiftRight(ref accumulator, ref f);          // LSR A
            AddWithCarry(ref accumulator, squareY, ref f);  // ADC square_y
            accumulator ^= squareX;                             // EOR square_x
            And(ref accumulator, 0x07, ref f);            // AND #$07

L18C1:
            f.Carry = false;                                    // CLC
            AddWithCarry(ref accumulator, 0x1d, ref f);     // ADC #$1D
            byte pushedAccumulator = accumulator;               // PHA
            some_background_calc_thing();                       // JSR some_background_calc_thing
            accumulator = pushedAccumulator;                    // PLA
            if (!f.Carry) goto return_background_empty;         // BCC return_background_empty
            yreg = accumulator;                                 // TAY
            accumulator = BackgroundLookup[yreg];              // LDA background_lookup,Y
            yreg = squareY;                                     // LDY square_y
            Compare(yreg, 0xE0, ref f);       // CPY #$E0
            if (!f.Zero) goto L18D7;                            // BNE L18D7
            accumulator ^= 0x40;                                // EOR #$40

L18D7:
            result.Result = accumulator;
            return result;                                      // RTS

return_background_empty:
            yreg = 0;                                           // LDY #$00

background_is_114f_lookup_with_y:
            f.Carry = true;                                     // SEC
            result.Result = BackgroundLookup[yreg];              // LDA background_lookup,Y
            return result;                                       // RTS

L18DF:
            some_background_calc_thing();                       // JSR some_background_calc_thing
            if (f.Carry) goto background_is_114f_lookup_with_top_of_9d;    // BCS background_is_114f_lookup_with_top_of_9d
            Compare(yreg, 0x0, ref f);      // CPY #$00
            if (f.Zero) goto background_is_114f_lookup_with_y;  // BEQ background_is_114f_lookup_with_y                ; empty space
            accumulator = zp_various_9d;                        // LDA zp_various_9d
            pushedAccumulator = accumulator;                    // PHA
            zp_various_9d = yreg;                               // STY zp_various_9c
            RotateLeft(ref accumulator, ref f);                 // ROL A
            RotateLeft(ref accumulator, ref f);                 // ROL A
            RotateLeft(ref accumulator, ref f);                 // ROL A
            And(ref accumulator, 0x01, ref f);          // AND #$01
            RotateLeft(ref accumulator, ref f);                 // ROL A
            yreg = accumulator;                                 // TAY
            accumulator = pushedAccumulator;                    // PLA
            AddWithCarry(ref accumulator, squareX, ref f);  // ADC square_x
            RotateLeft(ref accumulator, ref f);                 // ROL A
            accumulator ^= squareY;                             // EOR square_y
            And(ref accumulator, 0x1a, ref f);          // AND #$1A
            if (!f.Zero) goto L1913;                            // BNE L1913
            accumulator = yreg;                                 // TYA
            yreg = zp_various_9d;                               // LDY zp_various_9c
            accumulator ^= BackgroundLookup[yreg + 7];         // EOR background_lookup+7,Y
            And(ref accumulator, 0x7f, ref f);          // AND #$7F
            Compare(accumulator, 0x40, ref f);  // CMP #$40
            RotateLeft(ref accumulator, ref f);                 // ROL A
            And(ref accumulator, 0x07, ref f);          // AND #$07
            xreg = accumulator;                                 // TAX
            accumulator = BackgroundLookup[xreg + 17];         // LDA background_lookup+17,X
            accumulator ^= BackgroundLookup[yreg + 7];         // EOR background_lookup+7,Y
            result.Result = accumulator;
            return result;                                     // RTS

L1913:
            accumulator = BackgroundLookup[13 + yreg];         // LDA background_lookup+13,Y
            yreg = zp_various_9d;                               // LDY zp_various_9c
            accumulator ^= BackgroundLookup[7 + yreg];         // EOR background_lookup+7,Y
            result.Result = accumulator;
            return result;                                      // RTS

background_is_114f_lookup_with_top_of_9d:
            accumulator = zp_various_9d;                        // LDA zp_various_9d
            LogicalShiftRight(ref accumulator, ref f);          // LSR A
            LogicalShiftRight(ref accumulator, ref f);          // LSR A
            LogicalShiftRight(ref accumulator, ref f);          // LSR A
            And(ref accumulator, 0x0E, ref f);           // AND #$0E
            LogicalShiftRight(ref accumulator, ref f);          // LSR A
            AddWithCarry(ref accumulator, 0x01, ref f);     // ADC #$01
            yreg = accumulator;                                 // TAY

via_background_is_114f_lookup_with_y:
            goto background_is_114f_lookup_with_y;              // JMP background_is_114f_lookup_with_y

L192A:
            AddWithCarry(ref accumulator, squareX, ref f);  // ADC square_x
            RotateLeft(ref accumulator, ref f);                 // ROL A
            RotateLeft(ref accumulator, ref f);                 // ROL A
            RotateLeft(ref accumulator, ref f);                 // ROL A
            And(ref accumulator, 0x02, ref f);            // AND #$02
            AddWithCarry(ref accumulator, 0x19, ref f);     // ADC #$19
            yreg = accumulator;                                 // TAY
            goto background_is_114f_lookup_with_y;              // JMP background_is_114f_lookup_with_y

L1937:
            yreg = 0x19;                                        // LDY #$19
            accumulator = squareX;                              // LDA square_x
            LogicalShiftRight(ref accumulator, ref f);          // LSR A
            AddWithCarry(ref accumulator, squareX, ref f);  // ADC square_x
            And(ref accumulator, 0x17, ref f);            // AND #$17
            if (!f.Zero) goto L192A;                            // BNE L192A
            RotateRight(ref zp_various_9d, ref f);              // ROR zp_various_9d
            RotateRight(ref accumulator, ref f);                // ROR A
            result.Result = accumulator;
            return result;                                      // RTS

void some_background_calc_thing()
    {
    accumulator = xreg;                             // TXA
    LogicalShiftRight(ref accumulator, ref f);      // LSR A
    accumulator ^= squareY;                         // EOR square_y
    And(ref accumulator, 0x06, ref f);       // AND #$06
    if (!f.Zero) goto L1971;                        // BNE L1971
    accumulator = yreg;                             // TYA
    yreg = 0x02;                                    // LDY #$02
    And(ref accumulator, 0x20, ref f);          // AND #$20
    ArithmeticShiftLeft(ref accumulator, ref f);    // ASL A
    ArithmeticShiftLeft(ref accumulator, ref f);    // ASL A
    accumulator ^= 0xE5;                            // EOR #$E5
    byte opcode = accumulator;                      // STA L1961 ; self modifying code
    if (f.Negative) goto L195E;                     // BMI L195E
    yreg = 0x4;                                     // LDY #$04

L195E:
    accumulator = xreg;                             // TXA
    AddWithCarry(ref accumulator, 0x16, ref f); // ADC #$16
switch (opcode)                                                             // modified above
        {
        case 0x65: 
            AddWithCarry(ref accumulator, squareX, ref f);   // ADC square_x
            break;
        case 0xE5:
            SubtractWithBorrow(ref accumulator, squareX, ref f);    // SBC square_x
            break;
        default: throw new InvalidOperationException("no handler for opcode " + opcode.ToString("X"));
        }
    And(ref accumulator, 0x5F, ref f);          // AND #$5F
    xreg = accumulator;                             // TAX
    xreg -=1;                                       // DEX
    Compare(xreg, 0x0C, ref f);     // CPX #$0C
    if (!f.Carry) goto L19A3;                       // BCC L19A3
    if (f.Zero) goto L19A5;                         // BEQ L19A5
    yreg += 1;                                      // INY
    xreg += 1;                                      // INX
    if (xreg == 0) goto L19A5;                      // BEQ L19A5

L1971:
    accumulator = squareX;                             // LDA square_x
    LogicalShiftRight(ref accumulator, ref f);          // LSR A
    LogicalShiftRight(ref accumulator, ref f);          // LSR A
    LogicalShiftRight(ref accumulator, ref f);          // LSR A
    LogicalShiftRight(ref accumulator, ref f);          // LSR A
    if (f.Carry) goto L19A2;                        // BCS L19A2
    accumulator = 0x01;                             // LDA #$01
    AddWithCarry(ref accumulator, squareX, ref f);  // ADC square_x
    AddWithCarry(ref accumulator, squareY, ref f);  // ADC square_y
    And(ref accumulator, 0x8F, ref f);          // AND #$8F
    Compare(accumulator, 0x01, ref f);  // CMP #$01
    if (f.Zero) goto L19A3;                             // BEQ L19A3
    xreg = accumulator;                                 // TAX
    f.Carry = true;                                     // SEC
    accumulator = squareY;                              // LDA square_y
    SubtractWithBorrow(ref accumulator, squareX, ref f);    // SBC square_x
    And(ref accumulator, 0x2F, ref f);          // AND #$2F
    Compare(accumulator, 0x01, ref f);  // CMP #$01
    if (f.Zero) goto L19A3;                             // BEQ L19A3
    yreg = 2;                                           // LDY #$02
    Compare(accumulator, 0x02, ref f);  // CMP #$02
    if (f.Zero) goto L19A5;                             // BEQ L19A5
    yreg += 1;                                          // INY
    if (!f.Carry) goto L19A5;                           // BCC L19A5
    yreg += 1;                                          // INY
    Compare(xreg, 0x02, ref f);         // CPX #$02
    if (f.Zero) goto L19A5;                             // BEQ L19A5
    yreg += 1;                                          // INY
    if (!f.Carry) goto L19A5;                           // BCC L19A5

L19A2:
    return;                                             // RTS

L19A3:
    yreg = 0;                                           // LDY #$00

L19A5:
    f.Carry = false;                                    // CLC
    return;                                             // RTS
    }

            }

        public (byte background, int? backgroundObjectId, bool isHashDefault) GetMapData(byte calculatedBackground, byte squareX)
            {
            var spriteOrHash = calculatedBackground & 0x3f;
            if (spriteOrHash >= 9)
                {
                return (background: calculatedBackground, backgroundObjectId: null, isHashDefault: false);
                }

            // check the background objects X lookup list
            var listOfX = BackgroundObjectsXLookup[spriteOrHash];
            var positionInHash = Array.IndexOf(listOfX, squareX);
            if (positionInHash == -1)
                {
                byte defaultBackground = LookupForUnmatchedHash[spriteOrHash];
                defaultBackground ^= (byte) (spriteOrHash & 0xc0);
                return (background: defaultBackground, backgroundObjectId: null, isHashDefault: true);
                }

            var handlerList = BackgroundObjectsHandlerLookup[spriteOrHash];
            byte result = handlerList[positionInHash];
            int backgroundObjectId = positionInHash;
            for (int i = 0; i < spriteOrHash; i++)
                {
                var countOfItemsInHash = BackgroundObjectsXLookup[i].Length;
                backgroundObjectId += countOfItemsInHash;
                }
            return (background: result, backgroundObjectId: backgroundObjectId, isHashDefault: false);
            }

        public static string GetBackgroundEventTypeName(byte background)
            {
            var spriteOrBackgroundHandler = background & 0x3f;
            if (spriteOrBackgroundHandler < 0x10)
                {
                switch (spriteOrBackgroundHandler)
                    {
                    case 0: 
                        return "handle_background_invisible_switch";
                    case 1:
                        return "handle_background_teleport_beam";
                    case 2:
                        return "handle_background_object_from_data";
                    case 3:
                        return "handle_background_door";
                    case 4:
                        return "handle_background_stone_door";
                    case 5:
                    case 6:
                    case 7:
                        return "handle_background_object_from_type";
                    case 8:
                        return "handle_background_switch";
                    case 9:
                    case 0xA:
                        return "handle_background_object_emerging";
                    case 0xB:
                        return "handle_background_object_fixed_wind";
                    case 0xC:
                        return "handle_background_engine_thruster";
                    case 0xD:
                        return "handle_background_object_water";
                    case 0xE:
                        return "handle_background_object_random_wind";
                    case 0xF:
                        return "handle_background_mushrooms";
                    }
                }

            return null;
            }

        private static void AddWithCarry(ref byte accumulator, byte data, ref Flags flags)
            {
            int result = accumulator + data + (flags.Carry ? 1 : 0);
            accumulator = (byte) (result % 256);
            flags.Carry = result > 255;
            }

        private static void SubtractWithBorrow(ref byte accumulator, byte data, ref Flags flags)
            {
            int result = accumulator - data - (flags.Carry ? 0 : 1);
            if (result < 0)
                {
                flags.Carry = false;
                result += 256;
                }
            else
                {
                flags.Carry = true;
                }
            accumulator = (byte) (result % 256);
            }

        private static void LogicalShiftRight(ref byte accumulator, ref Flags flags)
            {
            flags.Carry = (accumulator & 1) != 0;
            accumulator = (byte) (accumulator >> 1);
            }

        private static void ArithmeticShiftLeft(ref byte accumulator, ref Flags flags)
            {
            flags.Carry = (accumulator & 0x80) != 0;
            accumulator = (byte) (accumulator << 1);
            }    

        private static void RotateLeft(ref byte accumulator, ref Flags flags)
            {
            bool currentCarryValue = flags.Carry;
            flags.Carry = (accumulator & 0x80) != 0;
            accumulator <<= 1;
            if (currentCarryValue)
                accumulator |= 1;
            flags.Zero = (accumulator == 0);
            flags.Negative = (accumulator & 0x80) != 0;
            }

        private static void RotateRight(ref byte data, ref Flags flags)
            {
            bool currentCarryValue = flags.Carry;
            flags.Carry = (data & 0x1) != 0;
            data >>= 1;
            if (currentCarryValue)
                data |= 0x80;
            flags.Zero = (data == 0);
            flags.Negative = (data & 0x80) != 0;
            }

        private static void Compare(byte registerValue, byte operand, ref Flags flags)
            {
            byte result = (byte) (registerValue - operand);
            flags.Carry = registerValue >= operand;
            flags.Zero = registerValue == operand;
            flags.Negative = (result & 0x80) != 0;
            }

        private static void And(ref byte accumulator, byte operand, ref Flags flags)
            {
            accumulator &= operand;
            flags.Zero = (accumulator == 0);
            flags.Negative = (accumulator & 0x80) != 0;
            }

        private static void Or(ref byte accumulator, byte operand, ref Flags flags)
            {
            accumulator |= operand;
            flags.Zero = (accumulator == 0);
            flags.Negative = (accumulator & 0x80) != 0;
            }

        private static void BitTest(byte accumulator, byte operand, ref Flags flags)
            {
            byte result = (byte) (accumulator & operand);
            flags.Zero = result == 0;
            flags.Negative = (operand & 0x80) != 0;
            flags.Overflow = (operand & 0x40) != 0;
            }

        private static byte[] BuildMapData()
            {
            var result = new byte[]
                {
                0x95,0xB6,0x19,0xEF,0x6F,0x6E,0x70,0x5E,0xD4,0xA9,0xA9,0x57,0x6D,0x06,0x6E,0xED,
                0x2D,0x6E,0x6E,0x06,0xCA,0x70,0xAD,0x07,0x5E,0x5E,0x53,0x62,0x53,0x9B,0x35,0x9E,
                0x15,0x16,0xE9,0x22,0x57,0x97,0x0C,0xCC,0x8C,0x78,0x3F,0xBD,0x05,0xED,0xE2,0x0A,
                0xF0,0x05,0x2D,0x6E,0xD3,0x07,0xE4,0x24,0x63,0xA1,0xA5,0x64,0x53,0x07,0xA4,0x63,
                0x66,0x7E,0x3E,0xDC,0x8C,0x72,0xE8,0xBC,0x06,0x19,0x22,0x6D,0xDE,0xD3,0x19,0x71,
                0xF1,0x7E,0x29,0xF4,0x39,0xA9,0xD3,0x06,0x53,0xA1,0xE4,0x07,0xD4,0xA9,0xD3,0x1A,
                0xC1,0x77,0xD7,0x41,0x6F,0xA1,0x6D,0x53,0xF5,0xD3,0x21,0x19,0xA1,0x53,0x06,0xE5,
                0xEE,0x19,0x97,0xD3,0x13,0xEA,0x75,0x02,0xD3,0x9B,0x53,0xEA,0x5F,0x85,0x72,0x21,
                0x6E,0x2C,0x2D,0x07,0xAD,0xED,0xB1,0x25,0x19,0x2F,0x53,0x3B,0x9E,0xE2,0xD3,0x62,
                0x02,0xF0,0x2D,0x06,0xA4,0xD3,0x19,0x21,0x53,0x21,0xED,0x30,0xD3,0x6A,0x59,0xA4,
                0x6D,0x70,0x6F,0x04,0xA4,0x64,0xA2,0xA2,0x1E,0x04,0xD3,0x01,0x4A,0x3B,0x64,0x63,
                0xF0,0x2D,0x17,0xED,0xF4,0x2F,0x12,0x30,0xD3,0x21,0xFA,0xA2,0xA1,0xE2,0x8D,0x2E,
                0x64,0x6E,0x02,0xEE,0x04,0x05,0x13,0xEE,0x4A,0x6A,0x2D,0x05,0x9B,0x2D,0x25,0x65,
                0xED,0xFE,0x31,0x6F,0xF0,0x14,0xEE,0xBF,0xDF,0x8D,0xD3,0x6A,0xDE,0x53,0x8B,0x1E,
                0xEE,0xAD,0x70,0x7A,0x24,0xA1,0x22,0x6D,0x22,0xD3,0x21,0x93,0xDF,0x01,0x02,0xDC,
                0xAE,0x7C,0x06,0xAF,0xDF,0xB2,0x07,0x29,0x03,0x5E,0xCD,0xEA,0x53,0xCD,0x07,0x8F,
                0xFC,0x94,0x66,0x69,0x30,0x07,0x62,0x35,0xD6,0x9D,0xBF,0x2F,0x9D,0x62,0x62,0x1F,
                0x53,0x21,0xD3,0x43,0xFE,0x45,0x93,0x74,0x9E,0xF0,0x91,0xAE,0xA1,0x62,0x02,0x07,
                0x6A,0xCC,0xD9,0x3D,0xE2,0xED,0xED,0xB0,0xB4,0x15,0xE6,0x19,0x57,0x17,0x9D,0x4C,
                0xED,0xA2,0x93,0x65,0x03,0x21,0x9E,0x05,0xB4,0xB0,0x06,0xEE,0x5E,0xA1,0x5E,0x25,
                0x49,0xF9,0x07,0x7C,0xDE,0xDE,0xEA,0x07,0x67,0x04,0xBD,0x68,0x53,0xCC,0x26,0xA8,
                0x7A,0x21,0xDE,0xE2,0x9E,0x06,0x53,0xA1,0x1E,0xE2,0x04,0xE8,0x9E,0x04,0x64,0x06,
                0xB9,0x06,0xDA,0x13,0xE3,0x4A,0x21,0xF8,0x05,0xC2,0x32,0x97,0x07,0x62,0xED,0x70,
                0xEF,0xEA,0xD3,0x6A,0xE4,0x19,0xC6,0xF3,0x03,0x19,0xA8,0x1E,0x28,0x9E,0xF5,0x29,
                0x07,0x04,0x70,0x21,0x1E,0x1E,0x06,0xFA,0xEE,0x2C,0x2D,0xF0,0x13,0x53,0xBB,0xF0,
                0x56,0x21,0xED,0xA1,0xAA,0xC0,0xC4,0x53,0x62,0xEF,0x2F,0xF0,0x70,0x5E,0xA1,0x19,
                0x6F,0xDE,0x1E,0xA1,0x24,0x02,0x5F,0x62,0x6D,0x06,0x71,0x8D,0x13,0x71,0xB0,0xAF,
                0x56,0xEA,0xDE,0xA5,0x21,0xE5,0x4B,0x8D,0x03,0x2F,0x29,0x2D,0x57,0x38,0x6E,0x07,
                0xD3,0x19,0x2A,0xE3,0xB5,0x6E,0x49,0xE5,0x70,0x62,0xB0,0x12,0x53,0xD3,0x22,0x6D,
                0xDF,0x8D,0x53,0xA1,0xD4,0xDF,0x21,0x1E,0x2D,0xF0,0x22,0x70,0x6E,0x35,0x12,0xE2,
                0x9A,0x23,0xA1,0x61,0x68,0x05,0xA5,0xD3,0x04,0x2E,0x06,0x19,0x07,0xD3,0xE1,0x2E,
                0x24,0x9B,0x53,0xCD,0x07,0xCD,0xCA,0x0F,0x52,0xED,0xE2,0x2E,0x05,0x34,0x78,0x04,
                0x3A,0x7B,0x04,0xAD,0x53,0xE1,0xB1,0x07,0xDF,0x21,0x13,0xFA,0x7E,0x19,0x5E,0x7B,
                0x05,0x96,0x3F,0xBD,0x54,0xDD,0x19,0xB1,0x32,0xBC,0x69,0x2B,0x21,0x6F,0xEE,0x19,
                0xB8,0xB2,0x2D,0x2D,0x64,0x20,0x53,0x03,0x53,0xA1,0x3E,0xFE,0xD3,0x07,0x53,0xFB,
                0xA8,0xB7,0x29,0x2B,0xE8,0xBC,0x68,0xDD,0x19,0x39,0xA2,0xB1,0xF0,0x53,0x1E,0xAD,
                0x70,0x3B,0x03,0xD6,0x53,0x1E,0x7A,0xA5,0x07,0x1B,0x53,0xDE,0x1E,0x9E,0xD3,0x21,
                0xD6,0x19,0x68,0xFD,0x02,0x6A,0x34,0x66,0xB0,0x9E,0x04,0xEF,0x04,0xDE,0xED,0xF1,
                0xED,0x18,0xA4,0x69,0x17,0x53,0x53,0xE2,0xED,0x30,0xDE,0xEA,0x9E,0x19,0x19,0x47,
                0xEF,0x06,0x8C,0x72,0xEF,0x19,0x2F,0xF0,0xED,0xB9,0x99,0xB1,0xDE,0x23,0xA3,0x78,
                0xA2,0x2F,0x30,0xEF,0x04,0xB5,0xE4,0xA1,0xD3,0x19,0x7A,0xA1,0xCD,0xDA,0x1B,0xD3,
                0x6D,0x06,0xED,0x71,0xB1,0x6E,0x04,0x6D,0xEE,0xAF,0x4A,0xD1,0x5E,0x1E,0x53,0x7C,
                0xEF,0x18,0xF0,0x2D,0x02,0xB8,0x7F,0x62,0x7C,0x8D,0x12,0xDE,0xC6,0xE4,0x8B,0xAE,
                0x0D,0xED,0xAD,0x8D,0xE2,0xDF,0xB1,0x6E,0xE4,0x04,0x64,0x07,0x25,0xF0,0xE4,0x19,
                0x2E,0xEF,0x19,0xB0,0x4F,0x32,0x75,0x07,0xE4,0x8D,0x5F,0x21,0xD4,0xCD,0xCB,0x53,
                0x8F,0xAE,0xAF,0xED,0x4A,0x21,0xA1,0x79,0x23,0xEA,0x07,0x13,0x54,0xF5,0x62,0x24,
                0x6F,0x4A,0xEE,0x11,0x13,0x93,0x1E,0xA9,0x25,0x5F,0xA1,0x7A,0x24,0xA5,0x5E,0x0F,
                0xA4,0x1E,0xEE,0x19,0xA0,0x53,0xE1,0xA1,0x93,0x93,0xD3,0xE1,0x32,0x97,0x93,0x53,
                0xD3,0x19,0x21,0x1E,0xF9,0x19,0xA5,0x03,0x6B,0x21,0xAE,0x12,0x7C,0x6A,0xFA,0x2D,
                0x38,0x72,0xBF,0xB0,0x21,0xEF,0x11,0xB5,0x56,0x36,0x02,0x3D,0x68,0x01,0x8C,0x30,
                0xD3,0x21,0x64,0x7E,0x64,0xA1,0x7C,0x21,0x54,0xFE,0xF2,0x6E,0xE4,0x29,0x5F,0x04,
                0x19,0x19,0x2A,0xAF,0x2D,0xE2,0x6A,0x6F,0xA7,0x69,0xF7,0xE9,0x32,0xA8,0xFC,0x28,
                0xFA,0x07,0xD3,0x6A,0x64,0x32,0x53,0x05,0x4A,0x62,0x04,0x56,0xD3,0x6A,0x54,0xA4,
                0x6D,0x53,0xE2,0xED,0x69,0x14,0x1E,0xEF,0x37,0x00,0x40,0x28,0xD3,0x05,0x2D,0x74,
                0xED,0x05,0xEF,0x5E,0x53,0xE3,0x19,0xA5,0x30,0x05,0x17,0xA1,0xA8,0x5F,0x21,0x05,
                0x22,0xED,0xE2,0xB1,0x62,0x02,0x64,0x65,0x6D,0x2C,0x12,0xCC,0x6D,0xE2,0x04,0xD3,
                0x53,0xA1,0x19,0xAB,0xA2,0xCD,0x8B,0x13,0x01,0xAF,0x21,0xED,0x51,0x94,0xF5,0x29,
                0x39,0x2E,0xF0,0x1A,0x5E,0x02,0x1E,0x7A,0xAD,0xED,0x39,0x70,0xB1,0xEE,0x03,0xB4,
                0xD6,0x8D,0x53,0x21,0xC2,0xDE,0x8B,0x2D,0xED,0x19,0x2F,0x2F,0x01,0xAF,0xFF,0x3F,
                0x53,0x00,0xED,0x25,0x06,0xEF,0x24,0xE2,0x2D,0xED,0xED,0xDE,0x5E,0x7B,0x31,0x07,
                0x13,0xCD,0xD3,0x1B,0xD4,0xCD,0x07,0x06,0xA2,0x6F,0xA2,0x31,0xF0,0x06,0xF8,0x62,
                0xA1,0x53,0xAA,0x00,0x64,0x05,0x00,0x25,0x0F,0x6D,0x53,0xED,0xD3,0x19,0x13,0x93,
                0x22,0xD3,0x22,0xE1,0x05,0x64,0x65,0x2D,0x70,0x19,0x62,0x06,0x22,0x63,0x63,0xBB,
                0x64,0x63,0x53,0x04,0x22,0x72,0x63,0x7E,0x64,0x63,0x64,0x05,0x22,0x65,0x5B,0xA1
                };
            return result;
            }

        private static Dictionary<int, byte[]> BuildBackgroundObjectsXLookup()
            {
            var result = new Dictionary<int, byte[]>
                {
                    {
                    0,
                    new byte[]
                        {
                        0xFF, 0xFF, 0xB0, 0xEC, 0x77, 0x64, 0x9A, 0xAF, 0xDA, 0xC6, 0x36, 0x9F, 0x2E, 0xA9,
                        0x9C, 0x83, 0x88, 0x5F, 0x57, 0xBF, 0x9D, 0x4D, 0x45, 0x81, 0xB3, 0x3F, 0xCB, 0x40, 0x4C
                        }
                    },
                    {
                    1,
                    new byte[]
                        {
                        0xCA, 0x2F, 0xA7, 0x56, 0x34, 0xE3, 0x3B, 0xE4, 0x80, 0xE0, 0x64, 0x37, 0x47, 0x9F,
                        0x9C, 0xAA, 0x9B, 0x9A, 0x5E, 0xC7, 0x8A, 0x60, 0x9D, 0xA2, 0xB2, 0x98, 0xA9, 0xDB
                        }
                    },
                    {
                    2,
                    new byte[]
                        {
                        0x28, 0x29, 0x3C, 0x98, 0x63, 0xCB, 0x61, 0xA3, 0xCE, 0xE9, 0x80, 0x2E, 0x4F, 0x79,
                        0x87, 0xB6, 0x97, 0x2D, 0xD6, 0x5C, 0xA0, 0x74, 0x6A, 0xA1, 0x9F, 0x89, 0x85, 0x6B,
                        0xAE, 0x65
                        }
                    },
                    {
                    3,
                    new byte[]
                        {
                        0xE2, 0xED, 0x80, 0xCD, 0xA8, 0x2B, 0xAB, 0x9D, 0x62, 0xE5, 0x70, 0xEC, 0x83, 0xC1,
                        0xC6, 0x67, 0xEB, 0x2D, 0x98, 0xAA, 0xCC, 0xA5, 0x9E, 0xA2, 0xD7, 0xE6, 0xE7, 0x94,
                        0x7C, 0xE3, 0x45, 0x9B, 0x9F, 0xC2, 0x71
                        }
                    },
                    {
                    4,
                    new byte[]
                        {
                        0x67, 0x4F, 0xCF, 0xD2, 0xE2, 0x7A, 0x62, 0xDA, 0x76, 0xB2, 0x66, 0xD7, 0x83, 0x84,
                        0x80, 0x87, 0x9B, 0x50, 0xAE, 0x64, 0xA3, 0x63, 0xB8, 0x7F, 0x82, 0xE0, 0x9C, 0x61,
                        0x9D, 0x29, 0x46, 0x9F, 0x9A, 0x74, 0x75, 0x77
                        }
                    },
                    {
                    5,
                    new byte[]
                        {
                        0xB2, 0xE4, 0x62, 0x63, 0x82, 0x61, 0xD4, 0xD3, 0x77, 0x2E, 0x64, 0x86, 0xA5, 0xA0,
                        0xD1, 0xB4, 0x7F, 0xA3, 0x9F, 0x99, 0x80, 0x67, 0xDA, 0x89, 0x95, 0x8B, 0xAB, 0xC4,
                        0x9D, 0xAA
                        }
                    },
                    {
                    6,
                    new byte[]
                        {
                        0xBB, 0x47, 0x8A, 0xA7, 0x61, 0x9E, 0x2E, 0xD6, 0x7E, 0xDA, 0xAA, 0xAB, 0x45, 0x67,
                        0xD4, 0x29, 0xB8, 0x6B, 0x69, 0x9D, 0x94, 0x63, 0xB4, 0xA1, 0x9F, 0xA0, 0x57, 0xE1
                        }
                    },
                    {
                    7,
                    new byte[]
                        {
                        0x7F, 0xA6, 0xB4, 0x53, 0x61, 0xD4, 0x82, 0xE3, 0x75, 0xC3, 0x84, 0x9E, 0xC6, 0x64,
                        0xA2, 0x28, 0x29, 0x9D, 0x83, 0xA8, 0x80, 0xAA, 0xD5, 0xA0, 0x9F, 0xD6, 0x62, 0x69,
                        0x2C, 0xA5
                        }
                    },
                    {8, new byte[] {0xB8, 0xB9, 0xD9, 0x59, 0x79, 0x39, 0x48, 0xE8}},
                    {9, new byte[] {0x03}}
                };
            return result;
            }

        private static byte[] BuildLookupForUnmatchedHash()
            {
            return new byte[] {0x1B,0x5A,0x19,0x19,0x1E,0x13,0x24,0x2C,0x19};
            }


        private static Dictionary<int, byte[]> BuildBackgroundObjectsHandlerLookup()
            {
            var result = new Dictionary<int, byte[]>
                {
                    {
                    0,
                    new byte[]
                        {
                        0x89, 0x89, 0x89, 0x89, 0x89, 0x8a, 0x46, 0xc6, 0x06, 0x06, 0x46, 0x05, 0x05, 0x00,
                        0xc3, 0x04, 0x83, 0x84, 0x84, 0x83, 0x88, 0x48, 0x02, 0x02, 0x42, 0x02, 0x7b, 0x22, 0x1e
                        }
                    },
                    {
                    1,
                    new byte[]
                        {
                        0x06, 0x06, 0xc6, 0x06, 0x46, 0x46, 0x85, 0x85, 0x87, 0x89, 0x89, 0xc7, 0x0a, 0x8c, 
                        0x03, 0x84, 0x83, 0x43, 0x84, 0x84, 0x02, 0x01, 0x41, 0x01, 0x01, 0x2e, 0x1e, 0x3b
                        }
                    },
                    {
                    2,
                    new byte[]
                        {
                        0x46, 0x46, 0x06, 0xc6, 0x06, 0x06, 0x06, 0x46, 0x06, 0x09, 0xc9, 0x89, 0xca, 0x8a,
                        0x8a, 0x0a, 0x4a, 0x8a, 0x04, 0x44, 0x41, 0x01, 0xc8, 0x48, 0xcc, 0x82, 0x02, 0x02,
                        0x02, 0x1e
                        }
                    },
                    {
                    3,
                    new byte[]
                        {
                        0x89, 0x09, 0x0a, 0x4a, 0xca, 0x4a, 0x86, 0x46, 0x46, 0x46, 0x86, 0x45, 0x47, 0x00,
                        0x00, 0x00, 0x00, 0x84, 0xc3, 0x44, 0x43, 0x43, 0x43, 0x44, 0x84, 0x44, 0x04, 0x01,
                        0x08, 0x08, 0x02, 0x02, 0x82, 0x1e, 0x2d
                        }
                    },
                    {
                    4,
                    new byte[]
                        {
                        0x46, 0x46, 0x45, 0x46, 0xc6, 0x89, 0xc9, 0x49, 0x89, 0xca, 0xca, 0x8a, 0x8a, 0x8a,
                        0x0a, 0x00, 0x00, 0xc4, 0x43, 0x04, 0x43, 0x84, 0x44, 0x04, 0x44, 0x84, 0x41, 0x01,
                        0x01, 0x01, 0xc8, 0x42, 0x82, 0x3b, 0x11, 0x3b
                        }
                    },
                    {
                    5,
                    new byte[]
                        {
                        0x89, 0xc9, 0xca, 0x8a, 0xc6, 0x06, 0xc6, 0xc6, 0x06, 0x06, 0x85, 0x47, 0x4a, 0xca,
                        0x8a, 0x00, 0x00, 0x44, 0x43, 0xc3, 0x44, 0x44, 0x04, 0x41, 0xc8, 0x88, 0xc8, 0xc8,
                        0x02, 0x8c
                        }
                    },
                    {
                    6,
                    new byte[]
                        {
                        0x49, 0x89, 0x0a, 0x0a, 0x8a, 0xc6, 0x06, 0x06, 0x47, 0x87, 0xcc, 0x41, 0x01, 0x08,
                        0x08, 0x08, 0x08, 0xc4, 0x84, 0x43, 0x43, 0x84, 0x04, 0x83, 0x82, 0x82, 0x0d, 0x0d
                        }
                    },
                    {
                    7,
                    new byte[]
                        {
                        0x46, 0x06, 0x06, 0x06, 0x06, 0x45, 0x45, 0x45, 0x06, 0x07, 0x89, 0x09, 0x8a, 0x4a,
                        0x4a, 0xca, 0xca, 0x4a, 0x00, 0x00, 0x00, 0x48, 0x08, 0x82, 0x82, 0x82, 0x02, 0xc4,
                        0xc4, 0x0b
                        }
                    },
                    {8, new byte[] {0x0b, 0x0b, 0xd1, 0x91, 0xd1, 0xd1, 0x91, 0x91}},
                    {9, new byte[] {0x0d}}
                };
            return result;
            }

        private static byte[] BuildBackgroundLookupTable()
            {
            return new byte[]
                {
                0x19, 0x2D, 0xED, 0x6D, 0xAD, 0x2D, 0xED,
                0x5E, 0x9E, 0x00, 0xC0, 0x80, 0x40,
                0x2E, 0x2F, 0x2E, 0x23, // [TOM 2f was 2e]
                0x06, 0x04, 0x06, 0x04, 0x07, 0x05, 0x05, 0x06, 0x19, 0x2C, 0x19, 0x2B, 0x00, 0x01, 0x02, 0x03,
                0x1A, 0x21, 0x09, 0x9B, 0x12, 0x10, 0x60, 0x2B, 0x0F, 0x4F, 0x04, 0x0A
                };
            }

        private struct Flags
            {
            public bool Carry;
            public bool Zero;
            public bool Negative;
            public bool Overflow;
            }
        }
    }
