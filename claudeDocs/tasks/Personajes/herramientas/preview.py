# Compone poses con las partes para revisar los cortes antes de llevarlos a Unity.
import json, os, sys
from PIL import Image

OUT = os.path.dirname(os.path.abspath(__file__)) + "/out"
ORDER = ["pierna_izq", "pierna_der", "brazo_izq", "brazo_der", "torso"]

# Ángulos en grados, positivos = antihorario en pantalla (como Unity Z).
POSES = {
    "reposo": {},
    "brazos_arriba": {"brazo_izq": -110, "brazo_der": 110},
    "golpe": {"brazo_izq": -150, "brazo_der": 150, "torso": 0},
    "paso": {"pierna_izq": 18, "pierna_der": -18, "brazo_izq": -18, "brazo_der": -18},
    "agacharse": {"_crouch": 0.12, "brazo_izq": 25, "brazo_der": -25},
    "senalar": {"brazo_der": 70},
}


def render(name, pose, bg=(200, 220, 200, 255)):
    man = json.load(open(os.path.join(OUT, name, "manifest.json")))
    w, h = man["size"]
    canvas = Image.new("RGBA", (w, h), bg)
    crouch = pose.get("_crouch", 0.0)
    for p in ORDER:
        info = man["parts"][p]
        part = Image.open(os.path.join(OUT, name, f"char_{name}_parte_{p}.png"))
        layer = Image.new("RGBA", (w, h), (0, 0, 0, 0))
        bb = info["bbox"]
        layer.paste(part, (bb[0], bb[1]))
        piv = info["pivot"]
        dy = 0
        if crouch and p.startswith("pierna"):
            # Aplasta la pierna hacia la cadera sin mover el pie: escala Y desde el suelo.
            floor = bb[3]
            s = 1 - crouch * 1.6
            hgt = floor - piv[1]
            sub = layer.crop((0, piv[1], w, floor)).resize((w, max(1, int(hgt * s))))
            layer2 = Image.new("RGBA", (w, h), (0, 0, 0, 0))
            layer2.paste(layer.crop((0, 0, w, piv[1])), (0, int(hgt * (1 - s))))
            layer2.alpha_composite(sub, (0, floor - sub.height))
            layer = layer2
        elif crouch:
            legs = man["parts"]["pierna_izq"]
            hgt = legs["bbox"][3] - legs["pivot"][1]
            dy = int(hgt * crouch * 1.6)
        ang = pose.get(p, 0)
        if ang:
            layer = layer.rotate(ang, center=(piv[0], piv[1]), resample=Image.BICUBIC)
        if dy:
            shifted = Image.new("RGBA", (w, h), (0, 0, 0, 0))
            shifted.paste(layer, (0, dy), layer)
            layer = shifted
        canvas.alpha_composite(layer)
    return canvas


if __name__ == "__main__":
    names = sys.argv[1:] or ["papa", "mama", "nina", "nino"]
    for name in names:
        tiles = [render(name, POSES[k]).resize((340, 340)) for k in POSES]
        sheet = Image.new("RGB", (340 * len(tiles), 340), (255, 255, 255))
        for i, t in enumerate(tiles):
            sheet.paste(t.convert("RGB"), (i * 340, 0))
        sheet.save(os.path.join(OUT, f"preview_{name}.png"))
        print("ok", name)
