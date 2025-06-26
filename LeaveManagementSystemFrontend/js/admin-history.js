const token = localStorage.getItem("jwtToken");
const role = localStorage.getItem("role");

if (!token || role !== "Admin") {
  alert("Unauthorized access.");
  window.location.href = "admin-login.html";
}

const toggle = document.getElementById("toggle");
const nav = document.getElementById("nav");
toggle.addEventListener("click", () => nav.classList.toggle("active"));

let allLeaves = [];
let currentPage = 1;
let itemsPerPage = 5;

async function searchLeaves() {
  const email = document.getElementById("searchInput").value.trim();
  if (!email) return;

  try {
    const encodedEmail = encodeURIComponent(email);
    const res = await fetch(
      `http://localhost:5000/api/Leaves/byEmail/${encodedEmail}`,

      {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      }
    );

    if (!res.ok) throw new Error("Leaves not found");

    allLeaves = await res.json();
    currentPage = 1;
    document.getElementById("resultTitle").innerText = `Leaves for ${email}`;
    renderLeaves();
  } catch (err) {
    document.getElementById("resultTitle").innerText = "";
    document.getElementById(
      "leaveResults"
    ).innerHTML = `<p style="color: red;">No leaves found.</p>`;
    document.getElementById("paginationControls").innerHTML = "";
  }
}

function formatDate(dateStr) {
  if (!dateStr) return "N/A";
  const date = new Date(dateStr);
  return isNaN(date.getTime()) ? "Invalid Date" : date.toLocaleDateString();
}

function renderLeaves() {
  const leaveResults = document.getElementById("leaveResults");
  const pagination = document.getElementById("paginationControls");
  leaveResults.innerHTML = "";
  pagination.innerHTML = "";

  const totalPages = Math.ceil(allLeaves.length / itemsPerPage);
  const start = (currentPage - 1) * itemsPerPage;
  const end = start + itemsPerPage;
  const leavesToShow = allLeaves.slice(start, end);

  if (leavesToShow.length === 0) {
    leaveResults.innerHTML = "<p>No leaves to show.</p>";
    return;
  }

  leavesToShow.forEach((leave) => {
    const card = document.createElement("div");
    card.className = "leave-card";

    const type = leave.leaveType || leave.type || "N/A";
    const startDate = leave.startDate || leave.start || null;
    const endDate = leave.endDate || leave.end || null;

    card.innerHTML = `
  <p><strong>Type:</strong> ${leave.leaveType || "N/A"}</p>
  <p><strong>Start:</strong> ${leave.startDateFormatted || "N/A"}</p>
  <p><strong>End:</strong> ${leave.endDateFormatted || "N/A"}</p>
  <p><strong>Reason:</strong> ${leave.reason || "N/A"}</p>
  <p><strong>Status:</strong> ${leave.status || "N/A"}</p>
`;
    leaveResults.appendChild(card);
  });

  // Prev button
  if (currentPage > 1) {
    const prev = document.createElement("button");
    prev.textContent = "Previous";
    prev.onclick = () => {
      currentPage--;
      renderLeaves();
    };
    pagination.appendChild(prev);
  }

  // Page numbers
  for (let i = 1; i <= totalPages; i++) {
    const pageBtn = document.createElement("button");
    pageBtn.textContent = i;
    pageBtn.classList.add("page-number");
    if (i === currentPage) pageBtn.classList.add("active-page");
    pageBtn.onclick = () => {
      currentPage = i;
      renderLeaves();
    };
    pagination.appendChild(pageBtn);
  }

  // Next button
  if (currentPage < totalPages) {
    const next = document.createElement("button");
    next.textContent = "Next";
    next.onclick = () => {
      currentPage++;
      renderLeaves();
    };
    pagination.appendChild(next);
  }
}

function changeItemsPerPage() {
  const selected = document.getElementById("itemsPerPage").value;
  itemsPerPage = parseInt(selected);
  currentPage = 1;
  renderLeaves();
}

//------------------------------------------------------------------------------------
document.getElementById("logoutBtn").addEventListener("click", function (e) {
  e.preventDefault();
  localStorage.clear();
  // Redirect to the appropriate login page
  window.location.href = "admin-login.html";
});
