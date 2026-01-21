import bpy
import bmesh
from mathutils import Vector

obj = bpy.context.object
me = obj.data

bm = bmesh.from_edit_mesh(me)
uv_layer = bm.loops.layers.uv.verify()

for face in bm.faces:
    if not face.select:
        continue

    # Face normal
    n = face.normal.normalized()

    # Build tangent space
    up = Vector((0, 0, 1))
    if abs(n.dot(up)) > 0.999:
        up = Vector((0, 1, 0))

    tangent = n.cross(up).normalized()
    bitangent = n.cross(tangent).normalized()

    # Use first vertex as origin
    origin = face.verts[0].co

    for loop in face.loops:
        v = loop.vert.co - origin

        u = v.dot(tangent)
        v2 = v.dot(bitangent)

        loop[uv_layer].uv = (u, v2)

bmesh.update_edit_mesh(me)
