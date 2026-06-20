using Godot;

public partial class Game : Node2D
{
    private readonly RandomNumberGenerator _rng = new();
    private float _time;

    public override void _Ready()
    {
        _rng.Randomize();

        SpawnAnimal(AnimalKind.Chicken, new Vector2(180, 300), 0.95f);
        SpawnAnimal(AnimalKind.Chicken, new Vector2(260, 390), 0.9f);
        SpawnAnimal(AnimalKind.Sheep, new Vector2(520, 330), 1.0f);
        SpawnAnimal(AnimalKind.Pig, new Vector2(700, 430), 0.92f);
        SpawnAnimal(AnimalKind.Cow, new Vector2(910, 360), 1.0f);
        SpawnAnimal(AnimalKind.Sheep, new Vector2(1080, 500), 0.86f);
    }

    public override void _Process(double delta)
    {
        _time += (float)delta;

        Rect2 playArea = GetPlayArea();
        foreach (Node child in GetChildren())
        {
            if (child is WanderAnimal animal)
                animal.SetPlayArea(playArea);
        }

        QueueRedraw();
    }

    public override void _Draw()
    {
        Vector2 size = GetViewportRect().Size;

        DrawRect(new Rect2(Vector2.Zero, size), new Color("#8fcf5b"));
        DrawRect(new Rect2(Vector2.Zero, new Vector2(size.X, Mathf.Min(130, size.Y * 0.22f))), new Color("#9bd8f0"));

        DrawSun(new Vector2(size.X - 86, 74));
        DrawFields(size);
        DrawFence(size);
        DrawPond(size);
        DrawBarn(size);
    }

    private void SpawnAnimal(AnimalKind kind, Vector2 position, float scale)
    {
        var animal = new WanderAnimal();
        animal.Configure(kind, GetPlayArea(), _rng.Randi(), scale);
        animal.Position = ClampToPlayArea(position);
        AddChild(animal);
    }

    private Rect2 GetPlayArea()
    {
        Vector2 size = GetViewportRect().Size;
        const float margin = 54f;
        float width = Mathf.Max(1, size.X - margin * 2f);
        float height = Mathf.Max(1, size.Y - margin * 2f);
        return new Rect2(new Vector2(margin, margin + 48f), new Vector2(width, Mathf.Max(1, height - 48f)));
    }

    private Vector2 ClampToPlayArea(Vector2 point)
    {
        Rect2 area = GetPlayArea();
        return new Vector2(
            Mathf.Clamp(point.X, area.Position.X, area.End.X),
            Mathf.Clamp(point.Y, area.Position.Y, area.End.Y)
        );
    }

    private void DrawSun(Vector2 center)
    {
        DrawCircle(center, 34f, new Color("#ffd166"));
        for (int i = 0; i < 12; i++)
        {
            float angle = Mathf.Tau * i / 12f;
            Vector2 ray = Vector2.Right.Rotated(angle);
            DrawLine(center + ray * 43f, center + ray * 58f, new Color("#ffd166"), 3f);
        }
    }

    private void DrawFields(Vector2 size)
    {
        Color rowA = new("#7ec850");
        Color rowB = new("#75bd49");

        for (float y = 132f; y < size.Y + 80f; y += 54f)
        {
            Color color = ((int)(y / 54f) % 2 == 0) ? rowA : rowB;
            DrawPolygon(
                new[]
                {
                    new Vector2(0, y),
                    new Vector2(size.X, y - 28f),
                    new Vector2(size.X, y + 28f),
                    new Vector2(0, y + 56f)
                },
                new[] { color }
            );
        }

        Color blade = new("#5aa238");
        for (float x = 30f; x < size.X; x += 86f)
        {
            float sway = Mathf.Sin(_time * 1.8f + x * 0.05f) * 3f;
            for (float y = 180f; y < size.Y; y += 118f)
            {
                DrawLine(new Vector2(x, y), new Vector2(x + sway, y - 13f), blade, 2f);
                DrawLine(new Vector2(x + 7f, y + 3f), new Vector2(x + 12f + sway, y - 8f), blade, 2f);
            }
        }
    }

    private void DrawFence(Vector2 size)
    {
        Color rail = new("#f4d09d");
        Color shadow = new("#b47a47");
        float bottom = size.Y - 76f;

        DrawLine(new Vector2(0, 142), new Vector2(size.X, 142), shadow, 5f);
        DrawLine(new Vector2(0, 158), new Vector2(size.X, 158), rail, 8f);
        DrawLine(new Vector2(0, bottom), new Vector2(size.X, bottom), shadow, 5f);
        DrawLine(new Vector2(0, bottom + 16f), new Vector2(size.X, bottom + 16f), rail, 8f);

        for (float x = -16f; x < size.X + 24f; x += 62f)
        {
            DrawRect(new Rect2(x, 122f, 14f, 72f), shadow);
            DrawRect(new Rect2(x + 2f, 118f, 10f, 68f), rail);
            DrawRect(new Rect2(x, bottom - 28f, 14f, 72f), shadow);
            DrawRect(new Rect2(x + 2f, bottom - 32f, 10f, 68f), rail);
        }
    }

    private void DrawPond(Vector2 size)
    {
        Vector2 center = new(Mathf.Min(190f, size.X * 0.2f), Mathf.Max(210f, size.Y - 150f));
        DrawOval(center, new Vector2(104f, 38f), new Color("#2e8fc9"));
        DrawOval(center + new Vector2(-12f, -5f), new Vector2(78f, 25f), new Color("#59b7de"));
        DrawLine(center + new Vector2(-55f, 0f), center + new Vector2(45f, 6f), new Color("#c7f6ff"), 3f);
        DrawLine(center + new Vector2(-26f, -14f), center + new Vector2(72f, -5f), new Color("#c7f6ff"), 2f);
    }

    private void DrawBarn(Vector2 size)
    {
        if (size.X < 620f || size.Y < 420f)
            return;

        Vector2 basePos = new(size.X - 260f, 154f);
        DrawRect(new Rect2(basePos, new Vector2(150f, 116f)), new Color("#b74336"));
        DrawPolygon(
            new[] { basePos + new Vector2(-18f, 0f), basePos + new Vector2(75f, -68f), basePos + new Vector2(168f, 0f) },
            new[] { new Color("#7f3129") }
        );
        DrawRect(new Rect2(basePos + new Vector2(54f, 54f), new Vector2(42f, 62f)), new Color("#63372a"));
        DrawLine(basePos + new Vector2(54f, 54f), basePos + new Vector2(96f, 116f), new Color("#f0c284"), 4f);
        DrawLine(basePos + new Vector2(96f, 54f), basePos + new Vector2(54f, 116f), new Color("#f0c284"), 4f);
        DrawRect(new Rect2(basePos + new Vector2(18f, 22f), new Vector2(34f, 28f)), new Color("#ffe4a6"));
    }

    private void DrawOval(Vector2 center, Vector2 radius, Color color)
    {
        DrawSetTransform(center, 0f, radius);
        DrawCircle(Vector2.Zero, 1f, color);
        DrawSetTransform(Vector2.Zero, 0f, Vector2.One);
    }
}

public enum AnimalKind
{
    Chicken,
    Sheep,
    Pig,
    Cow
}

public partial class WanderAnimal : Node2D
{
    private readonly RandomNumberGenerator _rng = new();

    private AnimalKind _kind;
    private Rect2 _playArea;
    private Vector2 _target;
    private Vector2 _velocity;
    private float _speed;
    private float _pauseTimer;
    private float _grazeTimer;
    private float _animationTime;
    private float _bodyScale = 1f;
    private bool _facingLeft;

    public void Configure(AnimalKind kind, Rect2 playArea, ulong seed, float bodyScale)
    {
        _kind = kind;
        _playArea = playArea;
        _rng.Seed = seed;
        _bodyScale = bodyScale;
        _speed = kind switch
        {
            AnimalKind.Chicken => 86f,
            AnimalKind.Sheep => 48f,
            AnimalKind.Pig => 56f,
            AnimalKind.Cow => 42f,
            _ => 52f
        } * _rng.RandfRange(0.82f, 1.18f);

        ZIndex = 20;
        PickNewTarget();
    }

    public void SetPlayArea(Rect2 playArea)
    {
        _playArea = playArea;
        Position = ClampToPlayArea(Position);
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;
        _animationTime += dt;

        if (_pauseTimer > 0f)
        {
            _pauseTimer -= dt;
            _velocity = _velocity.Lerp(Vector2.Zero, 8f * dt);
            if (_pauseTimer <= 0f)
                PickNewTarget();
        }
        else
        {
            Vector2 toTarget = _target - Position;
            if (toTarget.Length() < 14f)
            {
                StartPause();
            }
            else
            {
                Vector2 desired = toTarget.Normalized() * _speed;
                _velocity = _velocity.Lerp(desired, Mathf.Clamp(3.4f * dt, 0f, 1f));
                Position += _velocity * dt;

                if (Mathf.Abs(_velocity.X) > 4f)
                    _facingLeft = _velocity.X < 0f;
            }
        }

        if (_grazeTimer > 0f)
            _grazeTimer -= dt;

        Position = ClampToPlayArea(Position);
        ZIndex = 20 + (int)Position.Y;
        QueueRedraw();
    }

    public override void _Draw()
    {
        float movement = Mathf.Clamp(_velocity.Length() / Mathf.Max(_speed, 1f), 0f, 1f);
        float bob = Mathf.Sin(_animationTime * 10f) * 2.5f * movement;
        int face = _facingLeft ? -1 : 1;

        DrawShadow();

        switch (_kind)
        {
            case AnimalKind.Chicken:
                DrawChicken(face, bob);
                break;
            case AnimalKind.Sheep:
                DrawSheep(face, bob);
                break;
            case AnimalKind.Pig:
                DrawPig(face, bob);
                break;
            case AnimalKind.Cow:
                DrawCow(face, bob);
                break;
        }
    }

    private void PickNewTarget()
    {
        _target = new Vector2(
            _rng.RandfRange(_playArea.Position.X, _playArea.End.X),
            _rng.RandfRange(_playArea.Position.Y, _playArea.End.Y)
        );
    }

    private void StartPause()
    {
        _pauseTimer = _rng.RandfRange(0.35f, 1.25f);
        if (_rng.Randf() < 0.55f)
            _grazeTimer = _pauseTimer * _rng.RandfRange(0.65f, 1f);
    }

    private Vector2 ClampToPlayArea(Vector2 point)
    {
        return new Vector2(
            Mathf.Clamp(point.X, _playArea.Position.X, _playArea.End.X),
            Mathf.Clamp(point.Y, _playArea.Position.Y, _playArea.End.Y)
        );
    }

    private bool IsGrazing()
    {
        return _grazeTimer > 0f && _velocity.Length() < 12f;
    }

    private void DrawShadow()
    {
        DrawOval(new Vector2(0f, 26f * _bodyScale), new Vector2(34f, 10f) * _bodyScale, new Color(0f, 0f, 0f, 0.18f));
    }

    private void DrawChicken(int face, float bob)
    {
        float s = 0.82f * _bodyScale;
        float headDrop = IsGrazing() ? 14f : 0f;
        Color body = new("#fff7df");
        Color outline = new("#8b6f4e");

        DrawLeg(new Vector2(-9f, 16f) * s, bob, s);
        DrawLeg(new Vector2(8f, 15f) * s, -bob, s);
        DrawOval(new Vector2(-3f, bob) * s, new Vector2(25f, 20f) * s, outline);
        DrawOval(new Vector2(-3f, bob - 2f) * s, new Vector2(22f, 18f) * s, body);
        DrawOval(new Vector2(-9f, bob - 3f) * s, new Vector2(10f, 12f) * s, new Color("#f7d99b"));

        Vector2 head = new(face * 23f * s, (-17f + bob + headDrop) * s);
        DrawCircle(head, 12f * s, outline);
        DrawCircle(head + new Vector2(0f, -1.5f * s), 10f * s, body);
        DrawComb(head + new Vector2(-face * 2f * s, -12f * s), s);
        DrawBeak(head + new Vector2(face * 10f * s, 1f * s), face, s);
        DrawCircle(head + new Vector2(face * 4f * s, -4f * s), 1.8f * s, Colors.Black);
    }

    private void DrawSheep(int face, float bob)
    {
        float s = 1.0f * _bodyScale;
        float headDrop = IsGrazing() ? 18f : 0f;

        DrawLeg(new Vector2(-18f, 17f) * s, bob, s);
        DrawLeg(new Vector2(16f, 17f) * s, -bob, s);

        Color wool = new("#f6f0df");
        Color woolShade = new("#e2d9c3");
        for (int i = 0; i < 8; i++)
        {
            float angle = Mathf.Tau * i / 8f;
            Vector2 offset = new Vector2(Mathf.Cos(angle) * 18f, Mathf.Sin(angle) * 11f + bob) * s;
            DrawCircle(offset, 15f * s, woolShade);
            DrawCircle(offset + new Vector2(0f, -2f * s), 13f * s, wool);
        }

        DrawOval(new Vector2(0f, bob) * s, new Vector2(30f, 20f) * s, wool);

        Vector2 head = new(face * 33f * s, (-6f + bob + headDrop) * s);
        DrawOval(head, new Vector2(13f, 16f) * s, new Color("#6f5746"));
        DrawCircle(head + new Vector2(face * 4f * s, -5f * s), 2f * s, Colors.Black);
        DrawCircle(head + new Vector2(-face * 8f * s, -13f * s), 7f * s, wool);
    }

    private void DrawPig(int face, float bob)
    {
        float s = 0.96f * _bodyScale;
        float headDrop = IsGrazing() ? 12f : 0f;
        Color skin = new("#f5a3a8");
        Color shade = new("#d77482");

        DrawLeg(new Vector2(-18f, 17f) * s, bob, s, shade);
        DrawLeg(new Vector2(16f, 17f) * s, -bob, s, shade);
        DrawOval(new Vector2(0f, bob) * s, new Vector2(36f, 21f) * s, shade);
        DrawOval(new Vector2(-2f, bob - 2f) * s, new Vector2(33f, 19f) * s, skin);

        Vector2 head = new(face * 34f * s, (-8f + bob + headDrop) * s);
        DrawCircle(head, 17f * s, shade);
        DrawCircle(head + new Vector2(0f, -2f * s), 15f * s, skin);
        DrawOval(head + new Vector2(face * 11f * s, 4f * s), new Vector2(8f, 5f) * s, new Color("#e98391"));
        DrawCircle(head + new Vector2(face * 13f * s, 3f * s), 1.4f * s, new Color("#7b3f48"));
        DrawCircle(head + new Vector2(face * 4f * s, -6f * s), 2f * s, Colors.Black);
        DrawPolygon(
            new[] { head + new Vector2(-face * 8f, -13f) * s, head + new Vector2(-face * 17f, -21f) * s, head + new Vector2(-face * 1f, -19f) * s },
            new[] { shade }
        );

        Vector2 tail = new(-face * 34f * s, (-3f + bob) * s);
        DrawArc(tail, 8f * s, 0.3f, 5.2f, 16, shade, 3f * s);
    }

    private void DrawCow(int face, float bob)
    {
        float s = 1.08f * _bodyScale;
        float headDrop = IsGrazing() ? 14f : 0f;
        Color hide = new("#f8f2dc");
        Color spot = new("#30302e");
        Color outline = new("#5a4633");

        DrawLeg(new Vector2(-23f, 21f) * s, bob, s, outline);
        DrawLeg(new Vector2(23f, 21f) * s, -bob, s, outline);
        DrawOval(new Vector2(0f, bob) * s, new Vector2(45f, 25f) * s, outline);
        DrawOval(new Vector2(0f, bob - 2f) * s, new Vector2(42f, 22f) * s, hide);
        DrawOval(new Vector2(-14f, bob - 7f) * s, new Vector2(14f, 9f) * s, spot);
        DrawOval(new Vector2(16f, bob + 4f) * s, new Vector2(12f, 10f) * s, spot);

        Vector2 head = new(face * 45f * s, (-10f + bob + headDrop) * s);
        DrawOval(head, new Vector2(17f, 19f) * s, outline);
        DrawOval(head + new Vector2(0f, -2f * s), new Vector2(15f, 17f) * s, hide);
        DrawOval(head + new Vector2(face * 10f * s, 8f * s), new Vector2(10f, 6f) * s, new Color("#e7b4a0"));
        DrawCircle(head + new Vector2(face * 4f * s, -7f * s), 2f * s, Colors.Black);
        DrawHorn(head + new Vector2(-face * 8f * s, -16f * s), -face, s);
        DrawHorn(head + new Vector2(face * 8f * s, -16f * s), face, s);
        DrawLine(new Vector2(-face * 41f * s, (bob - 5f) * s), new Vector2(-face * 56f * s, (bob + 12f) * s), outline, 3f * s);
    }

    private void DrawLeg(Vector2 hip, float bob, float scale, Color? colorOverride = null)
    {
        Color color = colorOverride ?? new Color("#6b4f32");
        Vector2 knee = hip + new Vector2(1.5f, 11f + bob * 0.35f) * scale;
        Vector2 foot = knee + new Vector2(-2.5f, 10f) * scale;
        DrawLine(hip, knee, color, 4f * scale);
        DrawLine(knee, foot, color, 4f * scale);
    }

    private void DrawComb(Vector2 top, float scale)
    {
        Color comb = new("#d64545");
        DrawCircle(top, 4f * scale, comb);
        DrawCircle(top + new Vector2(4f * scale, -1f * scale), 4.5f * scale, comb);
        DrawCircle(top + new Vector2(-4f * scale, -1f * scale), 3.8f * scale, comb);
    }

    private void DrawBeak(Vector2 basePos, int face, float scale)
    {
        DrawPolygon(
            new[]
            {
                basePos + new Vector2(0f, -4f) * scale,
                basePos + new Vector2(face * 12f, 0f) * scale,
                basePos + new Vector2(0f, 5f) * scale
            },
            new[] { new Color("#f0a13a") }
        );
    }

    private void DrawHorn(Vector2 basePos, int face, float scale)
    {
        DrawPolygon(
            new[]
            {
                basePos,
                basePos + new Vector2(face * 8f, -10f) * scale,
                basePos + new Vector2(face * 2f, 4f) * scale
            },
            new[] { new Color("#ead7a2") }
        );
    }

    private void DrawOval(Vector2 center, Vector2 radius, Color color)
    {
        DrawSetTransform(center, 0f, radius);
        DrawCircle(Vector2.Zero, 1f, color);
        DrawSetTransform(Vector2.Zero, 0f, Vector2.One);
    }
}
