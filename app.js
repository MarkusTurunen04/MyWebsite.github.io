const STORAGE_KEY = "treenilingo-state-v2";

const challenges = [
  { title: "Haaste 1: Lämmittely", type: "pushup", target: 5, xp: 20 },
  { title: "Haaste 2: Voimapari", type: "pullup", target: 3, xp: 30 },
  { title: "Haaste 3: Tekniikkasetti", type: "pushup", target: 10, xp: 35 },
  { title: "Haaste 4: Leuanvetoporras", type: "pullup", target: 5, xp: 45 },
  { title: "Haaste 5: Viikon finaali", type: "pushup", target: 20, xp: 60 }
];

const baseState = {
  xp: 0,
  streak: 0,
  hearts: 5,
  current: 0
};

const state = loadState();

const els = {
  level: document.getElementById("level"),
  xp: document.getElementById("xp"),
  streak: document.getElementById("streak"),
  hearts: document.getElementById("hearts"),
  progress: document.getElementById("progress"),
  progressLabel: document.getElementById("progressLabel"),
  challengeTitle: document.getElementById("challengeTitle"),
  challengeDesc: document.getElementById("challengeDesc"),
  repInput: document.getElementById("repInput"),
  feedback: document.getElementById("feedback"),
  pathList: document.getElementById("pathList"),
  pushupBtn: document.getElementById("pushupBtn"),
  pullupBtn: document.getElementById("pullupBtn"),
  skipBtn: document.getElementById("skipBtn"),
  resetBtn: document.getElementById("resetBtn")
};

function challengeText(type) {
  return type === "pushup" ? "punnerrusta" : "leuanvetoa";
}

function currentLevel() {
  return Math.floor(state.xp / 100) + 1;
}

function levelProgress() {
  return state.xp % 100;
}

function description(challenge) {
  return `Tee vähintään ${challenge.target} ${challengeText(challenge.type)} hyvällä tekniikalla.`;
}

function setFeedback(text, kind = "ok") {
  els.feedback.textContent = text;
  els.feedback.classList.remove("ok", "warn");
  els.feedback.classList.add(kind);
}

function persist() {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(state));
}

function loadState() {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return { ...baseState };
    const parsed = JSON.parse(raw);
    return {
      xp: Number(parsed.xp) || 0,
      streak: Number(parsed.streak) || 0,
      hearts: Number(parsed.hearts) || 5,
      current: Number(parsed.current) || 0
    };
  } catch {
    return { ...baseState };
  }
}

function refreshPath() {
  els.pathList.innerHTML = "";
  challenges.forEach((challenge, index) => {
    const li = document.createElement("li");
    li.textContent = `${challenge.title} – ${challenge.target} ${challengeText(challenge.type)} (${challenge.xp} XP)`;

    if (index < state.current) li.classList.add("done");
    if (index === state.current) li.classList.add("current");

    els.pathList.appendChild(li);
  });
}

function toggleButtons(disabled) {
  els.pushupBtn.disabled = disabled;
  els.pullupBtn.disabled = disabled;
  els.skipBtn.disabled = disabled;
  els.repInput.disabled = disabled;
}

function render() {
  const level = currentLevel();
  const challenge = challenges[state.current];

  els.level.textContent = level;
  els.xp.textContent = state.xp;
  els.streak.textContent = `${state.streak} 🔥`;
  els.hearts.textContent = `${state.hearts} ❤️`;
  els.progress.value = levelProgress();
  els.progressLabel.textContent = `Edistyminen tasoon ${level + 1}`;

  if (!challenge) {
    els.challengeTitle.textContent = "🎉 Kaikki haasteet suoritettu!";
    els.challengeDesc.textContent = "Mahtavaa! Paina 'Aloita alusta' tehdäksesi uuden kierroksen.";
    toggleButtons(true);
  } else {
    els.challengeTitle.textContent = challenge.title;
    els.challengeDesc.textContent = description(challenge);
    toggleButtons(false);
  }

  refreshPath();
  persist();
}

function loseHeart(reason) {
  state.hearts -= 1;
  state.streak = 0;

  if (state.hearts <= 0) {
    state.hearts = 5;
    state.current = 0;
    setFeedback(`${reason} Sydämet loppuivat ja kierros alkoi alusta.`, "warn");
    render();
    return;
  }

  setFeedback(`${reason} Menetit yhden sydämen.`, "warn");
  render();
}

function completeChallenge(type) {
  const challenge = challenges[state.current];
  if (!challenge) return;

  const reps = Number(els.repInput.value);
  if (!Number.isFinite(reps) || reps < 0) {
    setFeedback("Anna kelvollinen toistomäärä.", "warn");
    return;
  }

  if (type !== challenge.type) {
    loseHeart("Valitsit väärän harjoitteen.");
    return;
  }

  if (reps < challenge.target) {
    loseHeart(`Toistoja liian vähän (${reps}/${challenge.target}).`);
    return;
  }

  state.xp += challenge.xp;
  state.streak += 1;
  state.current += 1;
  els.repInput.value = 0;
  setFeedback(`Loistavaa! Suoritit haasteen (+${challenge.xp} XP).`, "ok");
  render();
}

els.pushupBtn.addEventListener("click", () => completeChallenge("pushup"));
els.pullupBtn.addEventListener("click", () => completeChallenge("pullup"));
els.skipBtn.addEventListener("click", () => loseHeart("Ohitit haasteen."));
els.resetBtn.addEventListener("click", () => {
  Object.assign(state, baseState);
  els.repInput.value = 0;
  setFeedback("Sovellus nollattu. Aloitetaan uudestaan!", "ok");
  render();
});

render();
