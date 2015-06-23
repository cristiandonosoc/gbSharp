﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBSharp.CPU.Dictionaries
{
  class CPUCBInstructionLengths
  {
    internal static Dictionary<byte, byte> Setup()
    {
      return new Dictionary<byte, byte>() {
            {0x00, 2}, // RLC B
            {0x01, 2}, // RLC C
            {0x02, 2}, // RLC D
            {0x03, 2}, // RLC E
            {0x04, 2}, // RLC H
            {0x05, 2}, // RLC L
            {0x06, 2}, // RLC (HL)
            {0x07, 2}, // RLC A
            {0x08, 2}, // RRC B
            {0x09, 2}, // RRC C
            {0x0A, 2}, // RRC D
            {0x0B, 2}, // RRC E
            {0x0C, 2}, // RRC H
            {0x0D, 2}, // RRC L
            {0x0E, 2}, // RRC (HL)
            {0x0F, 2}, // RRC A
            {0x10, 2}, // RL B
            {0x11, 2}, // RL C
            {0x12, 2}, // RL D
            {0x13, 2}, // RL E
            {0x14, 2}, // RL H
            {0x15, 2}, // RL L
            {0x16, 2}, // RL (HL)
            {0x17, 2}, // RL A
            {0x18, 2}, // RR B
            {0x19, 2}, // RR C
            {0x1A, 2}, // RR D
            {0x1B, 2}, // RR E
            {0x1C, 2}, // RR H
            {0x1D, 2}, // RR L
            {0x1E, 2}, // RR (HL)
            {0x1F, 2}, // RR A
            {0x20, 2}, // SLA B
            {0x21, 2}, // SLA C
            {0x22, 2}, // SLA D
            {0x23, 2}, // SLA E
            {0x24, 2}, // SLA H
            {0x25, 2}, // SLA L
            {0x26, 2}, // SLA (HL)
            {0x27, 2}, // SLA A
            {0x28, 2}, // SRA B
            {0x29, 2}, // SRA C
            {0x2A, 2}, // SRA D
            {0x2B, 2}, // SRA E
            {0x2C, 2}, // SRA H
            {0x2D, 2}, // SRA L
            {0x2E, 2}, // SRA (HL)
            {0x2F, 2}, // SRA A
            {0x30, 2}, // SWAP B
            {0x31, 2}, // SWAP C
            {0x32, 2}, // SWAP D
            {0x33, 2}, // SWAP E
            {0x34, 2}, // SWAP H
            {0x35, 2}, // SWAP L
            {0x36, 2}, // SWAP (HL)
            {0x37, 2}, // SWAP A
            {0x38, 2}, // SRL B
            {0x39, 2}, // SRL C
            {0x3A, 2}, // SRL D
            {0x3B, 2}, // SRL E
            {0x3C, 2}, // SRL H
            {0x3D, 2}, // SRL L
            {0x3E, 2}, // SRL (HL)
            {0x3F, 2}, // SRL A
            {0x40, 2}, // BIT 0,B
            {0x41, 2}, // BIT 0,C
            {0x42, 2}, // BIT 0,D
            {0x43, 2}, // BIT 0,E
            {0x44, 2}, // BIT 0,H
            {0x45, 2}, // BIT 0,L
            {0x46, 2}, // BIT 0,(HL)
            {0x47, 2}, // BIT 0,A
            {0x48, 2}, // BIT 1,B
            {0x49, 2}, // BIT 1,C
            {0x4A, 2}, // BIT 1,D
            {0x4B, 2}, // BIT 1,E
            {0x4C, 2}, // BIT 1,H
            {0x4D, 2}, // BIT 1,L
            {0x4E, 2}, // BIT 1,(HL)
            {0x4F, 2}, // BIT 1,A
            {0x50, 2}, // BIT 2,B
            {0x51, 2}, // BIT 2,C
            {0x52, 2}, // BIT 2,D
            {0x53, 2}, // BIT 2,E
            {0x54, 2}, // BIT 2,H
            {0x55, 2}, // BIT 2,L
            {0x56, 2}, // BIT 2,(HL)
            {0x57, 2}, // BIT 2,A
            {0x58, 2}, // BIT 3,B
            {0x59, 2}, // BIT 3,C
            {0x5A, 2}, // BIT 3,D
            {0x5B, 2}, // BIT 3,E
            {0x5C, 2}, // BIT 3,H
            {0x5D, 2}, // BIT 3,L
            {0x5E, 2}, // BIT 3,(HL)
            {0x5F, 2}, // BIT 3,A
            {0x60, 2}, // BIT 4,B
            {0x61, 2}, // BIT 4,C
            {0x62, 2}, // BIT 4,D
            {0x63, 2}, // BIT 4,E
            {0x64, 2}, // BIT 4,H
            {0x65, 2}, // BIT 4,L
            {0x66, 2}, // BIT 4,(HL)
            {0x67, 2}, // BIT 4,A
            {0x68, 2}, // BIT 5,B
            {0x69, 2}, // BIT 5,C
            {0x6A, 2}, // BIT 5,D
            {0x6B, 2}, // BIT 5,E
            {0x6C, 2}, // BIT 5,H
            {0x6D, 2}, // BIT 5,L
            {0x6E, 2}, // BIT 5,(HL)
            {0x6F, 2}, // BIT 5,A
            {0x70, 2}, // BIT 6,B
            {0x71, 2}, // BIT 6,C
            {0x72, 2}, // BIT 6,D
            {0x73, 2}, // BIT 6,E
            {0x74, 2}, // BIT 6,H
            {0x75, 2}, // BIT 6,L
            {0x76, 2}, // BIT 6,(HL)
            {0x77, 2}, // BIT 6,A
            {0x78, 2}, // BIT 7,B
            {0x79, 2}, // BIT 7,C
            {0x7A, 2}, // BIT 7,D
            {0x7B, 2}, // BIT 7,E
            {0x7C, 2}, // BIT 7,H
            {0x7D, 2}, // BIT 7,L
            {0x7E, 2}, // BIT 7,(HL)
            {0x7F, 2}, // BIT 7,A
            {0x80, 2}, // RES 0,B
            {0x81, 2}, // RES 0,C
            {0x82, 2}, // RES 0,D
            {0x83, 2}, // RES 0,E
            {0x84, 2}, // RES 0,H
            {0x85, 2}, // RES 0,L
            {0x86, 2}, // RES 0,(HL)
            {0x87, 2}, // RES 0,A
            {0x88, 2}, // RES 1,B
            {0x89, 2}, // RES 1,C
            {0x8A, 2}, // RES 1,D
            {0x8B, 2}, // RES 1,E
            {0x8C, 2}, // RES 1,H
            {0x8D, 2}, // RES 1,L
            {0x8E, 2}, // RES 1,(HL)
            {0x8F, 2}, // RES 1,A
            {0x90, 2}, // RES 2,B
            {0x91, 2}, // RES 2,C
            {0x92, 2}, // RES 2,D
            {0x93, 2}, // RES 2,E
            {0x94, 2}, // RES 2,H
            {0x95, 2}, // RES 2,L
            {0x96, 2}, // RES 2,(HL)
            {0x97, 2}, // RES 2,A
            {0x98, 2}, // RES 3,B
            {0x99, 2}, // RES 3,C
            {0x9A, 2}, // RES 3,D
            {0x9B, 2}, // RES 3,E
            {0x9C, 2}, // RES 3,H
            {0x9D, 2}, // RES 3,L
            {0x9E, 2}, // RES 3,(HL)
            {0x9F, 2}, // RES 3,A
            {0xA0, 2}, // RES 4,B
            {0xA1, 2}, // RES 4,C
            {0xA2, 2}, // RES 4,D
            {0xA3, 2}, // RES 4,E
            {0xA4, 2}, // RES 4,H
            {0xA5, 2}, // RES 4,L
            {0xA6, 2}, // RES 4,(HL)
            {0xA7, 2}, // RES 4,A
            {0xA8, 2}, // RES 5,B
            {0xA9, 2}, // RES 5,C
            {0xAA, 2}, // RES 5,D
            {0xAB, 2}, // RES 5,E
            {0xAC, 2}, // RES 5,H
            {0xAD, 2}, // RES 5,L
            {0xAE, 2}, // RES 5,(HL)
            {0xAF, 2}, // RES 5,A
            {0xB0, 2}, // RES 6,B
            {0xB1, 2}, // RES 6,C
            {0xB2, 2}, // RES 6,D
            {0xB3, 2}, // RES 6,E
            {0xB4, 2}, // RES 6,H
            {0xB5, 2}, // RES 6,L
            {0xB6, 2}, // RES 6,(HL)
            {0xB7, 2}, // RES 6,A
            {0xB8, 2}, // RES 7,B
            {0xB9, 2}, // RES 7,C
            {0xBA, 2}, // RES 7,D
            {0xBB, 2}, // RES 7,E
            {0xBC, 2}, // RES 7,H
            {0xBD, 2}, // RES 7,L
            {0xBE, 2}, // RES 7,(HL)
            {0xBF, 2}, // RES 7,A
            {0xC0, 2}, // SET 0,B
            {0xC1, 2}, // SET 0,C
            {0xC2, 2}, // SET 0,D
            {0xC3, 2}, // SET 0,E
            {0xC4, 2}, // SET 0,H
            {0xC5, 2}, // SET 0,L
            {0xC6, 2}, // SET 0,(HL)
            {0xC7, 2}, // SET 0,A
            {0xC8, 2}, // SET 1,B
            {0xC9, 2}, // SET 1,C
            {0xCA, 2}, // SET 1,D
            {0xCB, 2}, // SET 1,E
            {0xCC, 2}, // SET 1,H
            {0xCD, 2}, // SET 1,L
            {0xCE, 2}, // SET 1,(HL)
            {0xCF, 2}, // SET 1,A
            {0xD0, 2}, // SET 2,B
            {0xD1, 2}, // SET 2,C
            {0xD2, 2}, // SET 2,D
            {0xD3, 2}, // SET 2,E
            {0xD4, 2}, // SET 2,H
            {0xD5, 2}, // SET 2,L
            {0xD6, 2}, // SET 2,(HL)
            {0xD7, 2}, // SET 2,A
            {0xD8, 2}, // SET 3,B
            {0xD9, 2}, // SET 3,C
            {0xDA, 2}, // SET 3,D
            {0xDB, 2}, // SET 3,E
            {0xDC, 2}, // SET 3,H
            {0xDD, 2}, // SET 3,L
            {0xDE, 2}, // SET 3,(HL)
            {0xDF, 2}, // SET 3,A
            {0xE0, 2}, // SET 4,B
            {0xE1, 2}, // SET 4,C
            {0xE2, 2}, // SET 4,D
            {0xE3, 2}, // SET 4,E
            {0xE4, 2}, // SET 4,H
            {0xE5, 2}, // SET 4,L
            {0xE6, 2}, // SET 4,(HL)
            {0xE7, 2}, // SET 4,A
            {0xE8, 2}, // SET 5,B
            {0xE9, 2}, // SET 5,C
            {0xEA, 2}, // SET 5,D
            {0xEB, 2}, // SET 5,E
            {0xEC, 2}, // SET 5,H
            {0xED, 2}, // SET 5,L
            {0xEE, 2}, // SET 5,(HL)
            {0xEF, 2}, // SET 5,A
            {0xF0, 2}, // SET 6,B
            {0xF1, 2}, // SET 6,C
            {0xF2, 2}, // SET 6,D
            {0xF3, 2}, // SET 6,E
            {0xF4, 2}, // SET 6,H
            {0xF5, 2}, // SET 6,L
            {0xF6, 2}, // SET 6,(HL)
            {0xF7, 2}, // SET 6,A
            {0xF8, 2}, // SET 7,B
            {0xF9, 2}, // SET 7,C
            {0xFA, 2}, // SET 7,D
            {0xFB, 2}, // SET 7,E
            {0xFC, 2}, // SET 7,H
            {0xFD, 2}, // SET 7,L
            {0xFE, 2}, // SET 7,(HL)
            {0xFF, 2} // SET 7,A
        };
    }
  }
}
