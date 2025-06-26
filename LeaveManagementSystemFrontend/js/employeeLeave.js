const token = localStorage.getItem("jwtToken");
const role = localStorage.getItem("role");

if (!token || role !== "Employee") {
  alert("Unauthorized access.");
  window.location.href = "employee-login.html";
}

// ---------------------------- Navigation Toggle ----------------------------------
const toggle = document.getElementById("toggle");
const nav = document.getElementById("nav");
toggle.addEventListener("click", () => nav.classList.toggle("active"));

// ---------------------------- Animated Labels ----------------------------------
const labels = document.querySelectorAll(".form-control label");
labels.forEach((label) => {
  label.innerHTML = label.innerText
    .split("")
    .map(
      (letter, idx) =>
        `<span style="transition-delay:${idx * 50}ms">${letter}</span>`
    )
    .join("");
});

// ---------------------------- DOMContentLoaded ----------------------------------
document.addEventListener("DOMContentLoaded", async () => {
  const email = localStorage.getItem("email");
  if (!email) {
    alert("You must log in first.");
    window.location.href = "employee-login.html";
    return;
  }

  const today = new Date().toISOString().split("T")[0];
  document.getElementById("startDate").setAttribute("min", today);
  document.getElementById("endDate").setAttribute("min", today);

  const emailField = document.getElementById("employeeEmailField");
  if (emailField) {
    emailField.value = email;
    emailField.readOnly = true;
  }

  try {
    const res = await fetch(
      `http://localhost:5000/api/Employees/byEmail/${email}`,
      {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      }
    );

    if (res.status === 401) {
      alert("Session expired. Please login again.");
      window.location.href = "employee-login.html";
      return;
    }

    if (!res.ok) throw new Error("Invalid user");

    const employee = await res.json();
    window.employeeId = employee.id;

    const employeeField = document.getElementById("employeeIdField");
    if (employeeField) {
      employeeField.value = employee.id;
      employeeField.readOnly = true;
    }
  } catch (err) {
    alert("Failed to fetch employee info.");
    console.error(err);
  }
});

// ----------------------------- Leave Form Submit ---------------------------------
document
  .getElementById("LeaveForm")
  .addEventListener("submit", async function (e) {
    e.preventDefault();

    const employeeId = window.employeeId;
    const leaveType = document.getElementById("leaveType").value.trim();
    const startDate = document.getElementById("startDate").value;
    const endDate = document.getElementById("endDate").value;
    const reason = document.getElementById("reason").value.trim();

    if (!employeeId || !leaveType || !startDate || !endDate || !reason) {
      alert("Please fill all fields.");
      return;
    }

    if (endDate < startDate) {
      alert("End date cannot be before start date.");
      return;
    }

    try {
      const res = await fetch(
        `http://localhost:5000/api/Leaves/byEmail/${localStorage.getItem(
          "email"
        )}`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      if (res.status === 401) {
        alert("Session expired. Please login again.");
        window.location.href = "employee-login.html";
        return;
      }

      let leaves = [];
      if (res.ok) {
        leaves = await res.json();
      } else if (res.status === 404) {
        leaves = [];
      } else {
        throw new Error("Unexpected response from server");
      }

      const newStart = new Date(startDate);
      const formattedNewStart = newStart.toISOString().split("T")[0];

      const hasConflict = leaves.some((leave) => {
        if (leave.status === "Rejected") return false;

        const existingStart = new Date(leave.startDate)
          .toISOString()
          .split("T")[0];

        return existingStart === formattedNewStart;
      });

      if (hasConflict) {
        alert(
          "You cannot apply for leave more than once on the same day. Apply again only if previous is rejected."
        );
        return;
      }

      const leaveData = {
        employeeId,
        leaveType,
        startDate,
        endDate,
        reason,
        status: "Pending",
      };

      const response = await fetch("http://localhost:5000/api/Leaves/apply", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(leaveData),
      });

      if (!response.ok) throw new Error("Failed to apply leave");

      alert("Leave Applied Successfully!");
      document.getElementById("LeaveForm").reset();
    } catch (error) {
      console.error("Leave Apply Error:", error);
      alert("Error applying for leave.");
    }
  });

// -------------------------------- Logout ---------------------------------
document.getElementById("logoutBtn").addEventListener("click", function (e) {
  e.preventDefault();
  localStorage.clear();
  window.location.href = "employee-login.html";
});
