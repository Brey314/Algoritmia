#!/usr/bin/env python3
# coreografia_v0.py: la coreografia ANTERIOR de los siete personajes (la que escribia a mano
# BuildRigsFinal.cs.txt hasta el 05/10/2026), portada tal cual a Python, y la REGRESION que la compara
# con los .anim del repo.
#
#     python3 claudeDocs/tasks/Personajes/herramientas/coreografia_v0.py
#
# Para que sirve: demostrar que el motor de coreografia.py (Spec, Follow, ClampedAuto, Crouch) es un
# port fiel del C#. Si reproduce los .anim que genero el C# con una diferencia maxima < 0,01 en el valor
# de cada clave, el motor se puede usar para la coreografia nueva con las mismas garantias.
# Solo vale contra .anim que se hayan generado con la coreografia anterior: tras volcar
# clips_personajes.json al repo, la comparacion fallara a proposito (los .anim ya seran los nuevos).
# Entonces este archivo se puede borrar.

import glob
import os
import sys

AQUI = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, AQUI)
import coreografia as K  # noqa: E402
import prefabs as P  # noqa: E402
from coreografia import (C, T, LA, RA, LE, RE, LL, RL, LK, RK, NK, HD, ROT, POSY, ESCX, ESCY, ALFA,  # noqa: E402
                         Spec, nuevo, cabeza_sigue, agacharse, guia_spec, cuelga, cuelga_piernas)


# ---------------------------------------------------------------------------- la familia: 21 clips


def human_clips(x):
    a = x.a
    lista = [
        idle(x),
        walk(x, "Walk", "caminar", 0.8, a, 8 * a, -4 * a, 1.5 * a, 10, 20, 0.015 * a),
        walk(x, "Run", "correr", 0.5, 1.6 * a, 20 * a, -6.5 * a, 2 * a, 50, 70, 0.04 * a),
        talk(x), strike(x), hammer(x), blow(x), pickup(x), kneel(x), carry(x), push(x), point(x), observe(x),
        celebrate(x), encourage(x), hug(x), surprise(x), sleep(x),
    ]
    visibilidad(x, lista)
    return lista


def idle(x):
    a = x.a
    s = nuevo(x, "Idle", "idle", 3.2)
    s.vol(0, 1.0, 1.28, 1.0 + 0.010 * a, 3.2, 1.0)
    s.rot(T, 0, 0, 0.8, 0.8 * a, 1.6, 0, 2.4, -0.8 * a, 3.2, 0)
    s.sym(LA, RA, 0, 0, 1.28, 1.5 * a, 3.2, 0)
    s.sym(LE, RE, 0, 0, 1.28, 1.0 * a, 3.2, 0)
    s.sym(LL, RL, 0, 0, 0.8, 0.6 * a, 1.6, 0, 2.4, -0.6 * a, 3.2, 0)
    s.rot(NK, 0, 0, 1.0, 0.9, 2.2, -0.6, 3.2, 0)
    s.follow(NK, ROT, HD, 3, 1.2)
    return s


def walk(x, accion, archivo, p, amp, vaiven, inclina, balanceo, flex_lo, flex_hi, aplasta):
    sube = 0.14 * x.pierna * amp
    medio = 0.5 * (flex_lo + flex_hi)
    s = nuevo(x, accion, archivo, p)
    s.raw(LL, POSY, 0, 0, 0.25 * p, sube, 0.5 * p, 0, p, 0)
    s.raw(RL, POSY, 0, 0, 0.5 * p, 0, 0.75 * p, sube, p, 0)
    s.rot(LL, 0, 0, 0.25 * p, -4 * amp, 0.5 * p, 0, p, 0)
    s.rot(RL, 0, 0, 0.5 * p, 0, 0.75 * p, 4 * amp, p, 0)
    s.rot(LK, 0, 0, 0.25 * p, 6 * amp, 0.5 * p, 0, p, 0)
    s.rot(RK, 0, 0, 0.5 * p, 0, 0.75 * p, -6 * amp, p, 0)
    if x.segmentado:
        s.raw(LK, ESCY, 0, 1, 0.25 * p, 0.9, 0.5 * p, 1, p, 1)
        s.raw(RK, ESCY, 0, 1, 0.5 * p, 1, 0.75 * p, 0.9, p, 1)
    s.raw(C, POSY, 0, 0, 0.25 * p, 10 * amp, 0.5 * p, 0, 0.75 * p, 10 * amp, p, 0)
    s.rot(C, 0, inclina, 0.25 * p, inclina - balanceo, 0.5 * p, inclina, 0.75 * p, inclina + balanceo, p, inclina)
    s.vol(0, 1 - aplasta, 0.25 * p, 1 + 0.7 * aplasta, 0.5 * p, 1 - aplasta, 0.75 * p, 1 + 0.7 * aplasta, p, 1 - aplasta)
    s.rot(T, 0, 0, 0.25 * p, 1.2 * amp, 0.5 * p, 0, 0.75 * p, -1.2 * amp, p, 0)
    s.rot(LA, 0, 0, 0.25 * p, vaiven, 0.5 * p, 0, 0.75 * p, -vaiven, p, 0)
    s.rot(RA, 0, 0, 0.25 * p, vaiven, 0.5 * p, 0, 0.75 * p, -vaiven, p, 0)
    s.rot(LE, 0, -medio, 0.25 * p, -flex_hi, 0.5 * p, -medio, 0.75 * p, -flex_lo, p, -medio)
    s.rot(RE, 0, medio, 0.25 * p, flex_lo, 0.5 * p, medio, 0.75 * p, flex_hi, p, medio)
    cabeza_sigue(s, C, ROT, -0.35, -0.2)
    return s


def talk(x):
    a = x.a
    s = nuevo(x, "Talk", "hablar", 1.6)
    s.vol(0, 1.0, 0.4, 1.012, 0.8, 1.0, 1.2, 1.012, 1.6, 1.0)
    s.rot(C, 0, 0, 0.8, 1.5, 1.6, 0)
    s.rot(T, 0, 0, 0.4, 1, 0.8, -1, 1.2, 1, 1.6, 0)
    s.rot(RA, 0, 0, 0.4, 28 * a, 0.8, 18 * a, 1.2, 30 * a, 1.6, 0)
    s.rot(RE, 0, 0, 0.4, 22 * a, 0.8, 12 * a, 1.2, 26 * a, 1.6, 0)
    s.rot(LA, 0, 0, 0.8, -5 * a, 1.6, 0)
    s.rot(LE, 0, 0, 0.8, -6 * a, 1.6, 0)
    s.sym(LL, RL, 0, 0, 0.8, 0.8 * a, 1.6, 0)
    s.rot(NK, 0, 0, 0.4, -2.5 * a, 0.6, 0.5, 1.2, -2.8 * a, 1.4, 0.5, 1.6, 0)
    s.follow(NK, ROT, HD, 3, 1.0)
    return s


def strike(x):
    s = nuevo(x, "Strike", "golpear", 0.6)
    s.rot(LA, 0, 40, 0.14, 22, 0.21, 12, 0.30, 62, 0.36, 52, 0.46, 42, 0.6, 40)
    s.rot(RA, 0, -40, 0.14, -22, 0.21, -12, 0.30, -62, 0.36, -52, 0.46, -42, 0.6, -40)
    s.sym(LE, RE, 0, -12, 0.21, -30, 0.30, -8, 0.36, -14, 0.6, -12)
    s.raw(T, POSY, 0, 0, 0.21, 5, 0.30, -8, 0.38, -3, 0.6, 0)
    s.vol(0, 1.0, 0.21, 1.025, 0.30, 0.965, 0.38, 1.01, 0.6, 1.0)
    s.rot(C, 0, 0, 0.21, 1.2, 0.30, -1.5, 0.6, 0)
    s.follow(T, POSY, NK, 2, 0.25)
    s.follow(T, POSY, HD, 4, 0.15)
    return s


def hammer(x):
    s = nuevo(x, "Hammer", "martillar", 0.7)
    s.rot(RA, 0, 0, 0.08, -8, 0.30, 125, 0.45, 112, 0.55, -15, 0.60, -8, 0.65, 5, 0.7, 0)
    s.rot(RE, 0, 6, 0.08, 14, 0.30, 70, 0.45, 72, 0.55, 4, 0.60, 10, 0.65, 14, 0.7, 6)
    s.rot(LA, 0, 20, 0.30, 24, 0.55, 28, 0.65, 21, 0.7, 20)
    s.rot(LE, 0, 14, 0.55, 20, 0.7, 14)
    s.rot(C, 0, 0, 0.30, 1.5, 0.55, -3, 0.65, 0.6, 0.7, 0)
    s.vol(0, 1.0, 0.30, 1.03, 0.55, 0.955, 0.62, 1.012, 0.7, 1.0)
    s.raw(T, POSY, 0, 0, 0.08, 2, 0.30, 5, 0.55, -6, 0.62, -1, 0.7, 0)
    cabeza_sigue(s, C, ROT, -0.5, -0.3)
    return s


def blow(x):
    s = nuevo(x, "Blow", "soplar", 0.9)
    d = agacharse(x, s, 0.3, 0, 8, 1.5, 0, 1, 0.9, 1)
    s.raw(T, POSY, 0, -d, 0.9, -d)
    s.vol(0, 1.0, 0.45, 0.95, 0.9, 1.0)
    s.rot(LA, 0, 35, 0.9, 35)
    s.rot(RA, 0, -35, 0.9, -35)
    s.sym(LE, RE, 0, -25, 0.45, -35, 0.9, -25)
    s.rot(T, 0, 0, 0.45, 1.5, 0.9, 0)
    s.rot(NK, 0, 2, 0.2, 5, 0.45, -3, 0.9, 2)
    s.follow(NK, ROT, HD, 3, 1.0)
    return s


def pickup(x):
    s = nuevo(x, "PickUp", "recoger", 1.4)
    d = agacharse(x, s, 0.35, 0, 10, 1.6, 0, 0, 0.4, 1, 0.7, 1, 1.1, 0, 1.4, 0)
    s.raw(T, POSY, 0, 0, 0.1, 4, 0.4, -d, 0.7, -d, 1.1, 3, 1.25, 0, 1.4, 0)
    s.rot(RA, 0, 0, 0.1, 8, 0.4, -25, 0.7, -25, 1.1, 40, 1.2, 44, 1.4, 0)
    s.rot(LA, 0, 0, 0.1, -4, 0.4, 20, 0.7, 20, 1.1, -10, 1.4, 0)
    s.rot(RE, 0, 0, 0.4, 22, 0.7, 22, 1.1, 45, 1.4, 0)
    s.rot(LE, 0, 0, 0.4, 12, 0.7, 12, 1.1, -8, 1.4, 0)
    s.vol(0, 1.0, 0.1, 1.02, 0.4, 0.965, 0.7, 0.97, 1.1, 1.04, 1.25, 1.01, 1.4, 1.0)
    s.rot(NK, 0, 0, 0.4, 5, 0.7, 5, 1.1, -2, 1.4, 0)
    s.follow(NK, ROT, HD, 3, 1.0)
    return s


def kneel(x):
    s = nuevo(x, "Kneel", "arrodillarse", 3.0)
    d = agacharse(x, s, 0.62, 14, 14, 2.2, 0, 1, 3, 1)
    s.raw(T, POSY, 0, -d, 3, -d)
    s.vol(0, 1.0, 1.5, 1.008, 3.0, 1.0)
    s.sym(LA, RA, 0, -22, 3, -22)
    s.sym(LE, RE, 0, -15, 1.5, -18, 3, -15)
    s.rot(T, 0, 0, 1.5, 0.8, 3, 0)
    s.rot(NK, 0, 3, 1.5, 4.5, 3, 3)
    s.follow(NK, ROT, HD, 4, 1.0)
    return s


def carry(x):
    a = x.a
    s = walk(x, "Carry", "cargar", 0.95, 0.8 * a, 0, -2 * a, 1.2 * a, 0, 0, 0.01 * a)
    s.drop(LA, ROT).drop(RA, ROT).drop(LE, ROT).drop(RE, ROT)
    s.sym(LA, RA, 0, 118, 0.475, 114, 0.95, 118)
    s.sym(LE, RE, 0, 14, 0.475, 20, 0.95, 14)
    return s


def push(x):
    a = x.a
    s = walk(x, "Push", "empujar", 0.9, 0.8 * a, 0, 0, 0, 0, 0, 0.01 * a)
    s.drop(LA, ROT).drop(RA, ROT).drop(LE, ROT).drop(RE, ROT).drop(C, ROT)
    s.sym(LA, RA, 0, -60, 0.9, -60)
    s.sym(LE, RE, 0, -22, 0.45, -26, 0.9, -22)
    s.rot(C, 0, -4, 0.45, -5, 0.9, -4)
    cabeza_sigue(s, C, ROT, -0.35, -0.2)
    return s


def point(x):
    s = nuevo(x, "Point", "senalar", 1.2)
    s.rot(RA, 0, 42, 0.3, 48, 0.6, 40, 0.9, 45, 1.2, 42)
    s.rot(RE, 0, 6, 0.3, 2, 0.6, 8, 0.9, 3, 1.2, 6)
    s.rot(LA, 0, 2, 0.6, 5, 1.2, 2)
    s.rot(LE, 0, -3, 0.6, -5, 1.2, -3)
    s.rot(C, 0, -3, 0.6, -3.5, 1.2, -3)
    s.rot(T, 0, -1.2, 0.6, -1.5, 1.2, -1.2)
    s.vol(0, 1.0, 0.6, 1.006, 1.2, 1.0)
    s.rot(NK, 0, -2.5, 0.6, -3.2, 1.2, -2.5)
    s.follow(NK, ROT, HD, 3, 1.0)
    return s


def observe(x):
    a = x.a
    s = nuevo(x, "Observe", "observar", 2.0)
    s.rot(C, 0, 5, 1, 8, 2, 5)
    s.sym(LA, RA, 0, -34, 2, -34)
    s.sym(LE, RE, 0, -40, 1, -46, 2, -40)
    s.vol(0, 1.0, 1, 1.008, 2, 1.0)
    s.rot(T, 0, 0, 1, -1.2 * a, 2, 0)
    s.sym(LL, RL, 0, 0, 1, 1.2 * a, 2, 0)
    s.rot(NK, 0, 0, 0.5, -6, 1, 0, 1.5, 6, 2, 0)
    s.follow(NK, ROT, HD, 4, 1.0)
    return s


def celebrate(x):
    a = x.a
    b = 0.06 * a
    s = nuevo(x, "Celebrate", "celebrar", 1.2)
    s.sym(LA, RA, 0, 104, 0.3, 86, 0.6, 104, 0.9, 86, 1.2, 104)
    s.sym(LE, RE, 0, 10, 0.3, 32, 0.6, 10, 0.9, 32, 1.2, 10)
    s.vol(0, 1.0, 0.15, 1 - b, 0.3, 1 + b, 0.6, 1.0, 0.75, 1 - b, 0.9, 1 + b, 1.2, 1.0)
    s.rot(C, 0, 0, 0.3, 1.5, 0.6, 0, 0.9, -1.5, 1.2, 0)
    s.rot(T, 0, 0, 0.3, -2, 0.6, 0, 0.9, 2, 1.2, 0)
    s.sym(LL, RL, 0, 0, 0.15, 3 * a, 0.3, 0, 0.6, 0, 0.75, 3 * a, 0.9, 0, 1.2, 0)
    s.sym(LK, RK, 0, 0, 0.15, -5 * a, 0.3, 0, 0.6, 0, 0.75, -5 * a, 0.9, 0, 1.2, 0)
    s.rot(NK, 0, 0, 0.3, 4, 0.6, 0, 0.9, -4, 1.2, 0)
    s.follow(NK, ROT, HD, 3, 1.2)
    return s


def encourage(x):
    s = nuevo(x, "Encourage", "animo", 0.9)
    s.rot(RA, 0, 110, 0.2, 95, 0.45, 128, 0.6, 120, 0.9, 110)
    s.rot(RE, 0, 40, 0.2, 70, 0.45, 25, 0.6, 35, 0.9, 40)
    s.rot(LA, 0, 3, 0.45, 8, 0.9, 3)
    s.rot(LE, 0, -4, 0.45, -8, 0.9, -4)
    s.vol(0, 1.0, 0.2, 0.97, 0.45, 1.04, 0.6, 1.0, 0.9, 1.0)
    s.rot(C, 0, 0, 0.45, -1.5, 0.9, 0)
    s.rot(NK, 0, 0, 0.2, -1.5, 0.45, 3, 0.9, 0)
    s.follow(NK, ROT, HD, 3, 1.1)
    return s


def hug(x):
    s = nuevo(x, "Hug", "abrazar", 2.0)
    s.sym(LA, RA, 0, 58, 1, 50, 2, 58)
    s.sym(LE, RE, 0, 38, 1, 30, 2, 38)
    s.rot(C, 0, -2.5, 1, 2.5, 2, -2.5)
    s.vol(0, 1.0, 1, 0.985, 2, 1.0)
    s.rot(T, 0, 0, 1, -1, 2, 0)
    cabeza_sigue(s, C, ROT, -1.0, -0.5)
    return s


def surprise(x):
    s = nuevo(x, "Surprise", "sorpresa", 1.0)
    s.sym(LA, RA, 0, 35, 0.25, 37.5, 0.5, 35, 0.75, 37.5, 1, 35)
    s.sym(LE, RE, 0, 12, 0.5, 16, 1, 12)
    s.vol(0, 1.03, 0.5, 1.04, 1, 1.03)
    s.sym(LL, RL, 0, 2.5, 0.5, 3, 1, 2.5)
    s.sym(LK, RK, 0, -3, 0.5, -3.5, 1, -3)
    s.rot(T, 0, 0, 0.5, -0.6, 1, 0)
    s.rot(NK, 0, -3, 0.5, -4, 1, -3)
    s.follow(NK, ROT, HD, 3, 1.0)
    return s


def sleep(x):
    s = nuevo(x, "Sleep", "dormir", 4.0)
    d = agacharse(x, s, 0.66, 16, 16, 2.0, 0, 1, 4, 1)
    s.raw(T, POSY, 0, -d, 4, -d)
    s.rot(T, 0, 16, 2, 17.5, 4, 16)
    s.vol(0, 1.0, 2, 1.025, 4, 1.0)
    s.rot(LA, 0, 12, 4, 12)
    s.rot(RA, 0, -38, 4, -38)
    s.rot(LE, 0, 10, 2, 12, 4, 10)
    s.rot(RE, 0, -28, 2, -30, 4, -28)
    s.rot(NK, 0, 8, 2, 9.5, 4, 8)
    s.follow(NK, ROT, HD, 4, 1.0)
    return s


def visibilidad(x, lista):
    a = x.a
    lista.append(nuevo(x, "Hidden", "oculto", 1.0).raw("Lienzo", ALFA, 0, 0.0, 1, 0.0))
    ap = nuevo(x, "Appear", "aparicion", 1.0, False)
    ap.raw("Lienzo", ALFA, 0, 0.0, 0.5, 1.0, 1, 1.0)
    ap.raw(C, ESCY, 0, 0.3, 0.6, 1.08, 0.8, 0.96, 1, 1.0)
    ap.raw(C, ESCX, 0, 0.3, 0.6, 0.98, 0.8, 1.04, 1, 1.0)
    ap.rot(C, 0, 0, 0.65, -2.5 * a, 0.8, 1.8 * a, 0.92, -0.7 * a, 1, 0)
    ap.sym(LA, RA, 0, 0, 0.7, 6, 0.85, -2, 1, 0)
    cabeza_sigue(ap, C, ROT, -0.8, -0.5)
    lista.append(ap)
    lista.append(nuevo(x, "Vanish", "apagado", 1.6, False)
                 .raw("Lienzo", ALFA, 0, 1.0, 0.3, 0.35, 0.6, 1.0, 0.9, 0.35, 1.2, 1.0, 1.6, 0.0)
                 .raw(C, ESCX, 0, 1.0, 1.2, 1.0, 1.6, 0.6)
                 .raw(C, ESCY, 0, 1.0, 1.2, 1.0, 1.6, 0.6)
                 .sym(LA, RA, 0, 0, 1.2, 0, 1.6, 10))


# ---------------------------------------------------------------------------- Algoritm: 9 clips


def guide_clips(x):
    lista = []
    idle_ = guia_spec("Idle", "flotar", 2.0)
    idle_.raw(C, POSY, 0, 0, 1, 24, 2, 0).rot(C, 0, -3, 1, 3, 2, -3)
    cuelga(idle_, 12)
    lista.append(idle_)

    talk_ = guia_spec("Talk", "hablar", 1.0)
    (talk_.raw(C, POSY, 0, 0, 0.5, 18, 1, 0)
     .raw(C, ESCX, 0, 1, 0.25, 1.05, 0.5, 1, 0.75, 1.05, 1, 1)
     .raw(C, ESCY, 0, 1, 0.25, 1.05, 0.5, 1, 0.75, 1.05, 1, 1)
     .rot(T, 0, 0, 0.25, 2, 0.5, 0, 0.75, -2, 1, 0)
     .raw(T, ESCY, 0, 1, 0.25, 1.03, 0.5, 1, 0.75, 1.03, 1, 1)
     .sym(LA, RA, 0, 0, 0.25, 6, 0.5, 0, 0.75, 6, 1, 0)
     .sym(LE, RE, 0, 0, 0.25, 8, 0.5, 0, 0.75, 8, 1, 0))
    cuelga_piernas(talk_, 9)
    lista.append(talk_)

    point_ = guia_spec("Point", "senalar", 1.2)
    (point_.rot(C, 0, -10, 0.6, -14, 1.2, -10).raw(C, POSY, 0, 10, 0.6, 24, 1.2, 10)
     .rot(RA, 0, 60, 0.6, 56, 1.2, 60)
     .rot(RE, 0, 3, 0.6, 6, 1.2, 3)
     .rot(LA, 0, 4, 0.6, 8, 1.2, 4)
     .rot(LE, 0, -4, 0.6, -6, 1.2, -4))
    cuelga_piernas(point_, 17)
    point_.follow(C, ROT, T, 2, 0.4)
    lista.append(point_)

    spin = guia_spec("Spin", "girar", 1.0)
    (spin.rot(C, 0, 0, 1, -360).raw(C, POSY, 0, 0, 0.5, 20, 1, 0)
     .sym(LA, RA, 0, 0, 0.15, 35, 0.85, 35, 1, 0)
     .sym(LE, RE, 0, 0, 0.2, 10, 0.8, 10, 1, 0)
     .sym(LL, RL, 0, 0, 0.15, 18, 0.85, 18, 1, 0)
     .sym(LK, RK, 0, 0, 0.2, -8, 0.8, -8, 1, 0))
    lista.append(spin)

    cel = guia_spec("Celebrate", "celebrar", 1.2)
    (cel.rot(C, 0, 0, 1.2, -360)
     .raw(C, ESCX, 0, 1, 0.3, 1.08, 0.6, 1, 0.9, 1.08, 1.2, 1)
     .raw(C, ESCY, 0, 1, 0.3, 1.08, 0.6, 1, 0.9, 1.08, 1.2, 1)
     .sym(LA, RA, 0, 100, 0.3, 85, 0.6, 100, 0.9, 85, 1.2, 100)
     .sym(LE, RE, 0, 12, 0.3, 30, 0.6, 12, 0.9, 30, 1.2, 12)
     .sym(LL, RL, 0, 10, 0.3, 18, 0.6, 10, 0.9, 18, 1.2, 10)
     .sym(LK, RK, 0, -6, 0.3, -14, 0.6, -6, 0.9, -14, 1.2, -6)
     .rot(T, 0, 0, 0.3, 2, 0.6, 0, 0.9, -2, 1.2, 0))
    lista.append(cel)

    enc = guia_spec("Encourage", "animo", 0.9)
    (enc.raw(C, POSY, 0, 0, 0.45, 30, 0.9, 0).raw(C, ESCY, 0, 1, 0.45, 1.06, 0.9, 1)
     .rot(RA, 0, 100, 0.2, 88, 0.45, 122, 0.6, 114, 0.9, 100)
     .rot(RE, 0, 40, 0.2, 64, 0.45, 24, 0.6, 34, 0.9, 40)
     .rot(LA, 0, 4, 0.45, 10, 0.9, 4)
     .rot(LE, 0, -4, 0.45, -8, 0.9, -4))
    cuelga_piernas(enc, 15)
    lista.append(enc)

    lista.append(guia_spec("Hidden", "oculto", 1.0).raw("Lienzo", ALFA, 0, 0.0, 1, 0.0))
    ap = guia_spec("Appear", "aparicion", 1.0, False)
    (ap.raw("Lienzo", ALFA, 0, 0.0, 0.5, 1.0, 1, 1.0)
     .raw(C, ESCX, 0, 0.3, 0.7, 1.06, 1, 1.0)
     .raw(C, ESCY, 0, 0.3, 0.7, 1.06, 1, 1.0)
     .sym(LA, RA, 0, 0, 0.7, 8, 0.85, -2, 1, 0))
    lista.append(ap)
    lista.append(guia_spec("Vanish", "apagado", 1.6, False)
                 .raw("Lienzo", ALFA, 0, 1.0, 0.3, 0.35, 0.6, 1.0, 0.9, 0.35, 1.2, 1.0, 1.6, 0.0)
                 .raw(C, ESCX, 0, 1.0, 1.2, 1.0, 1.6, 0.6)
                 .raw(C, ESCY, 0, 1.0, 1.2, 1.0, 1.6, 0.6))
    return lista


# ---------------------------------------------------------------------------- regresion contra los .anim


def leer_anim(ruta):
    """Lee m_EditorCurves de un .anim: {(ruta, atributo): [(t, v, inSlope, outSlope)]} y los ajustes del clip."""
    with open(ruta, encoding="utf-8") as f:
        lineas = f.read().split("\n")
    i = next(k for k, l in enumerate(lineas) if l.startswith("  m_EditorCurves:"))
    curvas = []
    cur = None
    k = i + 1
    while k < len(lineas) and (lineas[k].startswith("  -") or lineas[k].startswith("    ")):
        s = lineas[k].strip()
        if lineas[k].startswith("  - serializedVersion: 2"):
            cur = {"claves": [], "hecha": False}
            curvas.append(cur)
        elif s.startswith("- serializedVersion: 3"):
            cur["claves"].append({})
        elif cur is not None and not cur["hecha"] and cur["claves"] and s.split(":")[0] in ("time", "value", "inSlope", "outSlope"):
            cur["claves"][-1][s.split(":")[0]] = float(s.split(":")[1])
        elif s.startswith("attribute:"):
            cur["attr"] = s.split(":", 1)[1].strip()
        elif s.startswith("path:"):
            cur["ruta"] = s.split(":", 1)[1].strip()
            cur["hecha"] = True
        k += 1
    stop = next((float(l.split(":")[1]) for l in lineas if l.strip().startswith("m_StopTime:")), None)
    bucle = next((int(l.split(":")[1]) for l in lineas if l.strip().startswith("m_LoopTime:")), None)
    return {(c["ruta"], c["attr"]): [(q["time"], q["value"]) for q in c["claves"]] for c in curvas}, stop, bucle


def compara(spec, ruta_anim):
    """Compara un Spec (ya con finish()) con el .anim: devuelve (max |dv|, max |dt|, [problemas])."""
    reales, stop, _ = leer_anim(ruta_anim)
    probs = []
    max_dv = max_dt = 0.0
    propias = {(c.ruta, c.prop): c.claves for c in spec.orden}
    for clave in sorted(set(propias) | set(reales)):
        if clave not in propias:
            probs.append("solo en el .anim: %s %s" % clave)
            continue
        if clave not in reales:
            probs.append("solo en el port: %s %s" % clave)
            continue
        a, b = propias[clave], reales[clave]
        if len(a) != len(b):
            probs.append("%s %s: %d claves frente a %d" % (clave[0].split("/")[-1], clave[1], len(a), len(b)))
            continue
        for (t1, v1), (t2, v2) in zip(a, b):
            max_dt = max(max_dt, abs(t1 - t2))
            max_dv = max(max_dv, abs(v1 - v2))
    if stop is not None and abs(stop - spec.largo) > 1e-3:
        probs.append("m_StopTime %s frente a %s" % (stop, spec.largo))
    return max_dv, max_dt, probs


def regresion():
    rig = P.cargar_rig()
    peor = 0.0
    fallos = 0
    total = 0
    print("%-8s %-26s %8s %8s  %s" % ("pers.", "clip", "max dv", "max dt", "problemas"))
    for pid in P.FAMILIA + ["algoritm"]:
        if pid == "algoritm":
            x = K.leer_contexto_prefab("algoritm_fuego", rig)
            specs = guide_clips(x)
            carpeta = os.path.join(P.PERSONAJES_ARTE, "Algoritm", "Animations")
        else:
            x = K.leer_contexto_prefab(pid, rig)
            specs = human_clips(x)
            carpeta = os.path.join(P.PERSONAJES_ARTE, K.FAMILIA[pid].carpeta, "Animations")
        for s in specs:
            avisos = s.finish()
            ruta = os.path.join(carpeta, s.archivo + ".anim")
            total += 1
            if not os.path.isfile(ruta):
                print("%-8s %-26s FALTA el .anim" % (pid, s.archivo))
                fallos += 1
                continue
            dv, dt, probs = compara(s, ruta)
            peor = max(peor, dv)
            malo = dv >= 0.01 or dt > 1e-3 or probs or avisos
            fallos += 1 if malo else 0
            print("%-8s %-26s %8.5f %8.5f  %s" % (pid, s.archivo.split("_anim_")[1], dv, dt,
                                                   "; ".join(probs + avisos) if malo else ""))
    print("\n%d clips comparados, %d con diferencias; diferencia maxima en valores de clave: %.5f" % (total, fallos, peor))
    return 1 if fallos else 0


if __name__ == "__main__":
    sys.exit(regresion())
